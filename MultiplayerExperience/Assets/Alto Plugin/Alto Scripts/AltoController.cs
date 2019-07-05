 using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR;
using System.Collections;
using UnityEngine.Networking;

namespace Alto
{
    [RequireComponent(typeof(Rigidbody))]
    public class AltoController : NetworkBehaviour
    {
        private Rigidbody rigidBody;
        private Vector3 originPosition; //used to reset the Alto

        private bool setupComplete = false;

        ///This allows for control effects to be changed depending on the device being used, will be developed further!///
        public enum HMDDevice
        {
            HTC_Vive,
            Oculus_Rift,
            MR_HMD, //microsoft's MR headsets, made by various manufacturers & hololens
            Oculus_Mobile, //gearVR
            Daydream, //Google system
            Viveport, //Vive Focus - standalone HTC system
            Android_VR, //other mobile devices that don't use particular software (Google Play access)
            Android, //non VR mobile devices
            iOS, //non VR apple mobile devices
            Windows, //non VR Windows device
            Mac, //non VR apple device
            Linux //non VR linux device 
        }
        [HideInInspector]
        public HMDDevice currentDevice; 

        //controller for jetpack, use trigger
        [Tooltip("One controller to control the jetpack goes here")]
        public AltoJetPackController jetPackControl;
        //for oculus controls
        [HideInInspector]
        public SteamVR_Controller.Device oculusController;
        //for the speed UI on the left controller
        private List<Vector3> SpeedUIPosition;
        private List<Quaternion> SpeedUIRotation;

        [HideInInspector]
        public AltoHaptics altoHaptics;
        [Tooltip("The puck 3D model")]
        public GameObject AltoPuck;
        [Tooltip("The HUD on the HMD camera to show various Alto activities")]
        public HUDController HUDController;
        [TextArea]
        [Tooltip("Important")]
        public string Note = "These variables below are set in Alto.cfg \nPlease edit the config values in StreamingAssets instead";
        [Tooltip("Mulitplies normalised Magnitude Alto Input")]
        public float step = 10f;
        [Tooltip("Speed Alto moves based on the input from Alto itself")]
        public float AltoSpeed;
        [Tooltip("The incremental change caused by the user on the controller touchpad")]
        public float UserSpeedIncremenet;
        [Tooltip("The max speed mulitplier value the increment can reach")]
        public float maxSpeedMulitplier = 5.0f;
        [Tooltip("The min speed multiplier value the increment can reach")]
        public float minSpeedMulitplier = 0.25f;
        [Tooltip("Amount of force applied when jetpack control activated")]
        public float JetPackForce;
        [Tooltip("Adjustment for falling to compensate for slow falling rate")]
        public float FallingForceAdjustment;
        [HideInInspector]
        public bool hovering = false;
        private AudioSource altoHoverAudio;
        private bool stoppedAlto = false;

        private int userVelocity = 0; //used for haptics
        private float userSpeed = 0.5f; //used for user controlling their speed
        [Tooltip("angle correction for direction Alto is facing")]
        public float calibDirection;
        [Tooltip("direction to apply angle correction to Alto - usually Vector3.forward")]
        public Vector3 calibVector;
        [Tooltip("For calibrating in game - not implemented yet")]
        public bool isCalibrating = false;
       
        //these bools are for if the user has been off the Alto for a period of time to stop movement
        private bool checkingInactivity = false;
        private bool altoInactive = false;
        
        private float previousYPos = 0.0f;

        public enum AltoBoardState
        {
            Static, //does not move
            Dynamic, //can move on X and Z axis only
            DynamicJetPack //can move on X, Y and Z axis (can use jetpack controls)
        }
        [Tooltip("State Alto is in during runtime - to control how player can use Alto")]
        public AltoBoardState altoState;

        //getters for variables
        private Vector3 altoDirection = Vector3.zero;
        public Vector3 GetAltoDirection
        {
            get { return altoDirection; }
        }

        private float altoMagnitude = 0.0f;
        public float GetAltoMagnitude
        {
            get { return altoMagnitude; }
        }

        public float UserSpeed
        {
            get { return userSpeed; }
            set { userSpeed = value; }
        }

        void Awake()
        {
            if (!setupComplete)
            {
                InitializeAltoController();
            }
        }

        void Start()
        {
            if (!setupComplete)
            {
                InitializeAltoController();
            }
        }

        private void InitializeAltoController()
        {
            //was hoping to overwrite entire transform but read only have to set pos and rot separately
            SpeedUIPosition = new List<Vector3>();
            SpeedUIRotation = new List<Quaternion>();
            //Vive = 0, Oculus = 1, MR = 2, the same as the device enum values
            SpeedUIPosition.Add(new Vector3(0.0f, 0.01f, -0.052f));
            SpeedUIPosition.Add(new Vector3(0.018f, 0.007f, -0.05f));
            SpeedUIPosition.Add(new Vector3(0.0f, 0.009f, -0.051f));
            SpeedUIRotation.Add(Quaternion.Euler(270.0f, 0.0f, 0.0f));
            SpeedUIRotation.Add(Quaternion.Euler(305.0f, -1.0f, 13.5f));
            SpeedUIRotation.Add(Quaternion.Euler(285.0f, 0.0f, 0.0f));
            //TODO: add to this to cover all HMD's
            string device = XRDevice.model;
            int deviceIndex = -1;
            if (device.Contains("Oculus"))
            {
                currentDevice = HMDDevice.Oculus_Rift;
                //oculus buttons can only be accessed through SteamVR_Controller with openVR
                deviceIndex = (int)jetPackControl.controllerIndex; //if this controller device # is not set before play sometimes checks the class before it has started and returns 0
                if(deviceIndex == -1) { deviceIndex = 1; Debug.Log("unable to find device index, please make sure controllers are on. Index set to 1"); }
                oculusController = SteamVR_Controller.Input(deviceIndex);
            }
            else if (device.Contains("Vive"))
            {
                currentDevice = HMDDevice.HTC_Vive;
            }
            else
            {
                currentDevice = HMDDevice.MR_HMD;
            }
            //need the lists entries to match the device enum entries
            HUDController.ControllerSpeedUI.transform.localPosition = SpeedUIPosition[(int)currentDevice];
            HUDController.ControllerSpeedUI.transform.localRotation = SpeedUIRotation[(int)currentDevice];

            rigidBody = GetComponent<Rigidbody>();
            originPosition = transform.position;
            altoHaptics = gameObject.AddComponent<AltoHaptics>();
            //set variables from config
            rigidBody.mass = AltoInput.GetConfigFloat("Player", "Mass");
            rigidBody.drag = AltoInput.GetConfigFloat("Player", "Drag");
            rigidBody.angularDrag = AltoInput.GetConfigFloat("Player", "AngularDrag");
            step = AltoInput.GetConfigFloat("Player", "Step");
            userSpeed = AltoInput.GetConfigFloat("Player", "UserStartingSpeed");
            AltoSpeed = AltoInput.GetConfigFloat("Player", "AltoSpeed");
            UserSpeedIncremenet = AltoInput.GetConfigFloat("Player", "UserSpeedIncrement");
            JetPackForce = AltoInput.GetConfigFloat("Player", "JetPackForce");
            FallingForceAdjustment = AltoInput.GetConfigFloat("Player", "FallingForceAdjustment");
            calibDirection = AltoInput.GetConfigFloat("Alto", "Offset");
            altoHoverAudio = AltoPuck.GetComponent<AudioSource>();
            HUDController.SetSpeedRange(minSpeedMulitplier, maxSpeedMulitplier);
            setupComplete = true;
        }

        ///Alto functions///
        private void ProcessAltoInput()
        {
            //get input values from Input class
            float altoInputDirection = AltoInput.GetDirection();
            Debug.Log("AltoInputDirection = " + AltoInput.GetDirection());
            float altoInputMagnitudeNorm = AltoInput.GetMagnitudeNormalised();
            Debug.Log("AltoInputMagnitudeNorm = " + AltoInput.GetMagnitudeNormalised());

            //apply to HUD GUI 
            HUDController.UpdateMagnitude(altoInputMagnitudeNorm); 
            altoDirection = (Quaternion.AngleAxis(altoInputDirection - calibDirection + 90, Vector3.up)) * calibVector;

            altoMagnitude = step * altoInputMagnitudeNorm;
            Vector3 PuckRot = Quaternion.AngleAxis(altoInputDirection + 90, Vector3.up) * Vector3.forward;
            AltoPuck.transform.eulerAngles = PuckRot * (altoInputMagnitudeNorm * 20);
            HUDController.SetAltoInput(altoDirection, altoMagnitude);
        }

        private void AltoMovement()
        {
            if (!stoppedAlto)
            {
                rigidBody.AddForce(altoDirection * altoMagnitude * (AltoSpeed * userSpeed * Time.deltaTime));

                userVelocity = Mathf.RoundToInt(Mathf.Lerp(0, 9, rigidBody.velocity.magnitude / 6f));
            }
        }

        private void UpdateUserSpeed()
        {
            if(currentDevice == HMDDevice.Oculus_Rift)
            {
                //as per openVR API:
                //A/X = k_EButton_A
                //B/Y = k_EButton_ApplicationMenu
               if (oculusController.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
               {
                    if (userSpeed < maxSpeedMulitplier) { userSpeed += UserSpeedIncremenet; }
                    HUDController.UpdateSpeedUI(true);
               }
               else if (oculusController.GetPress(Valve.VR.EVRButtonId.k_EButton_A))
               {
                    if (userSpeed > minSpeedMulitplier) { userSpeed -= UserSpeedIncremenet; }
                    HUDController.UpdateSpeedUI(false);
                }
               else { HUDController.UpdateSpeedUI(false, false); }
            }
            else
            {
                if (jetPackControl.padPressed)
                {
                    Vector2 padPosition = jetPackControl.padPosition;
                    if (padPosition.x >= -0.8f && padPosition.x <= 0.8f)
                    {
                        if (padPosition.y > 0.5f)
                        {
                            if (userSpeed < maxSpeedMulitplier) { userSpeed += UserSpeedIncremenet; jetPackControl.HapticCurve(0.3f, 1100); }
                            HUDController.UpdateSpeedUI(true);
                        }
                        else if (padPosition.y < -0.5f)
                        {
                            if (userSpeed > minSpeedMulitplier) { userSpeed -= UserSpeedIncremenet; jetPackControl.HapticCurve(0.3f, 1100, false); }
                            HUDController.UpdateSpeedUI(false);
                        }
                        else { HUDController.UpdateSpeedUI(false, false); }
                    }
                    else { HUDController.UpdateSpeedUI(false, false); }
                }
                else
                { HUDController.UpdateSpeedUI(false, false); }
            }
            HUDController.UpdateSpeed(userSpeed);

            if(userSpeed <= minSpeedMulitplier)
            {
                stoppedAlto = true;
                StopAlto(false);
            }
            else
            {
                stoppedAlto = false;
            }
        }

        private void StopAlto(bool inactive = true)
        {
            if (inactive)
            {
                altoHaptics.ChangeHaptic(AltoHaptics.HapticState.None);
            }
            userVelocity = 0;
            rigidBody.velocity = new Vector3(0.0f, rigidBody.velocity.y, 0.0f);
            rigidBody.angularVelocity = new Vector3(0.0f, rigidBody.velocity.y, 0.0f);
        }

        private void ProcessAltoState()
        {
            //use state to control what movement can occur
            if (altoState != AltoBoardState.Static)
            {
                AltoMovement();
                UpdateUserSpeed();
                CheckHovering();

                if (altoState == AltoBoardState.DynamicJetPack)
                {
                    JetPackMovement(jetPackControl.triggerPressed);
                    CheckFalling();
                }
            }
            else
            {
                StopAlto(); 
            }
        }

        private void JetPackMovement(bool controllerEvent)
        {
            if (controllerEvent)
            {
                rigidBody.AddForce(new Vector3(0, (JetPackForce * step), 0));
                //controller haptics
                jetPackControl.HapticCurve(0.4f, 1000);
                if (!altoHoverAudio.isPlaying)
                {
                    altoHoverAudio.Play();
                }
            }
            else
            {
                altoHoverAudio.Stop();
            }
        }

        private void CheckFalling()
        {
            float difference = gameObject.transform.position.y - previousYPos;
            if (difference < 0) //falling
            {
                rigidBody.AddForce(new Vector3(0, (JetPackForce * step * FallingForceAdjustment), 0));
                if (HUDController.downArrowState == HUDController.ArrowState.Hide && RaycastCheck(2.0f) == "NT")
                {
                    HUDController.downArrowState = HUDController.ArrowState.Reveal;
                    HUDController.upArrowState = HUDController.ArrowState.Hide;
                }
            }
            else if (difference > 0)
            {
                if(HUDController.upArrowState == HUDController.ArrowState.Hide && jetPackControl.triggerPressed)
                {
                    HUDController.upArrowState = HUDController.ArrowState.Reveal;
                    HUDController.downArrowState = HUDController.ArrowState.Hide;
                } 
            }
            else 
            {
                if (!jetPackControl.triggerPressed)
                {
                    HUDController.upArrowState = HUDController.ArrowState.Hide;
                    HUDController.downArrowState = HUDController.ArrowState.Hide;
                }
            }
            previousYPos = gameObject.transform.position.y;
        }

        private string RaycastCheck(float distance)
        {
            RaycastHit raycast;
            if (Physics.Raycast(AltoPuck.transform.position, Vector3.down, out raycast, distance))
            {
                return raycast.transform.tag;
            }
            return "NT"; //for not touching
        }

        private void CheckHovering()
        {
            string objTag = RaycastCheck(0.75f);
            if(objTag == "NT")
            {
                hovering = true;
                altoHaptics.ChangeHaptic(AltoHaptics.HapticState.Hover);
            }
            else
            {
                hovering = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            string name = collision.transform.tag;
            AltoHaptics.HapticState newState = AltoHaptics.HapticState.None;
            switch (name)
            {
                case "Terrain":
                    newState = AltoHaptics.HapticState.Ground;
                    break;
                case "Water":
                    newState = AltoHaptics.HapticState.Water;
                    break;
                case "ImpactObject":
                    altoHaptics.ImpactHaptic(altoHaptics.GetHapticState());
                    break;
            }
            if (hovering || altoHaptics.GetHapticState() == AltoHaptics.HapticState.Hover)
            {
                altoHaptics.ImpactHaptic(newState);
            }
            else
            {
                altoHaptics.ChangeHaptic(newState);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            //TODO: was being used for a gun, needs to be moved to another script
        }

        //called externally by a game controller to change the state - currently called internally
        public void SetAltoState(string stateName)
        {
            switch (stateName)
            {
                case "Static":
                    altoState = AltoBoardState.Static;
                    break;
                case "Dynamic":
                    altoState = AltoBoardState.Dynamic;
                    break;
                case "JetPack":
                    altoState = AltoBoardState.DynamicJetPack;
                    altoHaptics.ImpactHaptic(altoHaptics.GetHapticState(), 2.0f);
                    break;
            }
        }
        //for learning GUI tutorial in game controller
        public void TutorialUpdateSpeedGUI(bool reset = false)
        {
            if (reset)
            {
                userSpeed = 0.0f;
                UserSpeed = 0.0f;
                HUDController.UpdateSpeedUI(false, false);

            }
            else
            {
                UpdateUserSpeed();
            }
        }

        private void CheckPlayerPresent()
        {
            if (altoMagnitude >= 0.75f)
            {
                if (checkingInactivity) { StopCoroutine(InactivePlayerCheck()); checkingInactivity = false; }
                altoInactive = false;
            }
            else { if (!checkingInactivity && !altoInactive) { checkingInactivity = true; StartCoroutine(InactivePlayerCheck()); } }
        }

        /// Timer coroutines ///
        
        IEnumerator InactivePlayerCheck()
        {
            yield return new WaitForSeconds(3.0f);
            checkingInactivity = false;
            StopAlto();
            altoInactive = true;
        }

        //called by game controller as cue to player alto is activated
        public void ActivateAltoHaptics(int intensity, float timer)
        {
            altoHaptics.ImpactHaptic(AltoHaptics.HapticState.None, 2.0f);
        }

        //accessed by game controller when need to reset the player
        public void ResetAlto()
        {
            StopAlto();
            altoState = AltoBoardState.Static;
            HUDController.ActivateGameUI();
            //TODO: smooth teleportation of Alto player to start position
            transform.position = originPosition;
            AltoInput.ResetAlto();
        }        

        void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }
            if (AltoInput.IsPresent())
            {
                ProcessAltoInput();
                if (!altoInactive)
                {
                    ProcessAltoState();
                }
                altoHaptics.RunHapticFeedback(userVelocity, altoMagnitude);
            }
            else
            {
                //Debug.Log("Cannot detect Alto, check port and Alto.cfg");
            }

        }
    }
}

