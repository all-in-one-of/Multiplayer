using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {

    private Image upArrow;
    private Image downArrow;
    private Image magnitude;
    private Image speed;
    private Transform direction;
    private Image directionArrow;
    private GameObject AltoUI;
    private GameObject TextUI;
    private GameObject GameUI;
    private Image AmmoUI;
    private Image AmmoIcon;
    private bool lowAmmoWarning = false;
    private TextMesh Score;
    private TextMesh TimerUI; //controlled in the game controller

    //For the controller speed button(s)
    public Canvas ControllerSpeedUI;
    public Sprite arrowFilled;
    private Sprite arrowEdge;
    private Image upSpeed;
    private Image downSpeed;

    private Vector3 altoDirection;
    private float altoMagnitude;
    private float minMagnitude = 1.0f;

    public float revealTime;

    private float maxSpeed = 0.0f;
    private float minSpeed = 0.0f;
    private float arrowStep = 0.0f;

    public enum ArrowState
    {
        Hide,
        Reveal,
        Visible,
    }
    public ArrowState upArrowState;
    public ArrowState downArrowState;

    //being called after game controller start() so wasn't set yet
    private bool started = false;

    private void Awake()
    {
        foreach (Transform ui in GetComponentsInChildren<Transform>())
        {
            if (ui.GetComponent<Image>() != null)
            {
                if (ui.name.Contains("Magnitude")) { magnitude = ui.GetComponent<Image>(); magnitude.fillAmount = 0.0f; }
                else if (ui.name.Contains("Speed")) { speed = ui.GetComponent<Image>(); speed.fillAmount = 0.0f; }
                else if (ui.name.Contains("TravelUp")) { upArrow = ui.GetComponent<Image>(); upArrow.fillAmount = 0.0f; }
                else if (ui.name.Contains("TravelDown")) { downArrow = ui.GetComponent<Image>(); downArrow.fillAmount = 0.0f; }
                else if (ui.name.Contains("Arrow")) { directionArrow = ui.GetComponent<Image>(); directionArrow.color = new Color(1.0f, 1.0f, 1.0f, 0.0f); }
                else if (ui.name.Contains("AmmoUI")) { AmmoUI = ui.GetComponent<Image>(); AmmoUI.fillAmount = 1.0f; }
                else if (ui.name.Contains("AmmoIcon")) { AmmoIcon = ui.GetComponent<Image>(); AmmoIcon.color = Color.white; }
                else { Debug.Log("Fund HUD child with no matching name: " + ui.name); }
            }
            else if (ui.name != "HUD") //don't need to find the HUD parent
            {
                if (ui.name.Contains("Direction")) { direction = ui; }
                else if (ui.name.Contains("AltoUI")) { AltoUI = ui.gameObject; }
                else if (ui.name.Contains("GameUI")) { GameUI = ui.gameObject; }
                else if (ui.name.Contains("UI Text")) { TextUI = ui.gameObject; }
                else if (ui.name.Contains("ScoreUI")) { Score = ui.GetComponent<TextMesh>(); }
                else if (ui.name.Contains("TimerUI")) { TimerUI = ui.GetComponent<TextMesh>(); }
                else { Debug.Log("Found HUD child with no matching name: " + ui.name); }
            }
        }
        upArrowState = ArrowState.Hide;
        downArrowState = ArrowState.Hide;
        arrowStep = revealTime / 1.0f;

        foreach (Image i in ControllerSpeedUI.GetComponentsInChildren<Image>())
        {
            if (i.name.Contains("Up")) { upSpeed = i; arrowEdge = i.sprite; }
            else if (i.name.Contains("Down")) { downSpeed = i; }
            else { Debug.Log("Found child of speed UI that doesn't match: " + i.name); }
        }
        GameUI.SetActive(false);
        started = true;
    }

    void Start ()
    {
        if (!started)
        {
            foreach (Transform ui in GetComponentsInChildren<Transform>())
            {
                if (ui.GetComponent<Image>() != null)
                {
                    if (ui.name.Contains("Magnitude")) { magnitude = ui.GetComponent<Image>(); magnitude.fillAmount = 0.0f; }
                    else if (ui.name.Contains("Speed")) { speed = ui.GetComponent<Image>(); speed.fillAmount = 0.0f; }
                    else if (ui.name.Contains("TravelUp")) { upArrow = ui.GetComponent<Image>(); upArrow.fillAmount = 0.0f; }
                    else if (ui.name.Contains("TravelDown")) { downArrow = ui.GetComponent<Image>(); downArrow.fillAmount = 0.0f; }
                    else if (ui.name.Contains("Arrow")) { directionArrow = ui.GetComponent<Image>(); directionArrow.color = new Color(1.0f, 1.0f, 1.0f, 0.0f); }
                    else { Debug.Log("Fund HUD child with no matching name: " + ui.name); }
                }
                else if (ui.name != "HUD") //don't need to find the HUD parent
                {
                    if (ui.name.Contains("Direction")) { direction = ui; }
                    else if (ui.name.Contains("AltoUI")) { AltoUI = ui.gameObject; Debug.Log(ui.name); }
                    else if (ui.name.Contains("GameUI")) { GameUI = ui.gameObject; }
                    else { Debug.Log("Found HUD child with no matching name: " + ui.name); }
                }
            }
            upArrowState = ArrowState.Hide;
            downArrowState = ArrowState.Hide;
            arrowStep = revealTime / 1.0f;

            foreach (Image i in ControllerSpeedUI.GetComponentsInChildren<Image>())
            {
                if (i.name.Contains("Up")) { upSpeed = i; arrowEdge = i.sprite; }
                else if (i.name.Contains("Down")) { downSpeed = i; }
                else { Debug.Log("Found child of speed UI that doesn't match: " + i.name); }
            }
            GameUI.SetActive(false);
            started = true;
        }
    }

    public void MoveUI(bool centre = true, float maxTime=1.0f)
    {
        if (centre)
        {
            AltoUI.transform.localPosition = new Vector3(220.0f, 40.0f, 0.0f);
            TextUI.transform.localPosition = new Vector3(0.0f, 60.0f, 0.0f);
            TextUI.transform.localRotation = Quaternion.Euler(-15.0f, 0.0f, 0.0f);
        }
        else
        {
            StartCoroutine(LerpUI(maxTime));
        }
    }

    private IEnumerator LerpUI(float maxTime)
    {
        float timer = 0.0f;
        float altoStep = 0.0f;
        float textStep = 0.0f;
        bool altoMoving = true;
        AnimationCurve animCurve = new AnimationCurve();
        animCurve.AddKey(new Keyframe(0, -1, 0, 0));
        animCurve.AddKey(new Keyframe(0.5f, 0, 0, 0));
        animCurve.AddKey(new Keyframe(1, 1, 0, 0));
        Vector3 startPosAlto = AltoUI.transform.localPosition;
        Vector3 startPosText = TextUI.transform.localPosition;
        Vector3 endPosText = new Vector3(0.0f, -150.0f, 0.0f);
        Quaternion startRotText = TextUI.transform.localRotation;
        Quaternion endRotText = Quaternion.Euler(25.0f, 0.0f, 0.0f);
        while (timer <= maxTime)
        {
            timer += Time.deltaTime;
            if (altoMoving)
            {
                if (altoStep >= 1.0f) { altoMoving = false; }
                else
                {
                    AltoUI.transform.localPosition = Vector3.Lerp(startPosAlto, Vector3.zero, animCurve.Evaluate(altoStep));
                    altoStep = timer / (maxTime / 2f);
                }
            }
            else
            {
                TextUI.transform.localPosition = Vector3.Lerp(startPosText, endPosText, animCurve.Evaluate(textStep));
                TextUI.transform.localRotation = Quaternion.Lerp(startRotText, endRotText, animCurve.Evaluate(textStep));
                textStep = timer / (maxTime / 2f) - 1.0f;
            }
            yield return null;
        }   
    }

    public void ActivateGameUI()
    {
        bool current = GameUI.activeSelf;
        GameUI.SetActive(!current);
    }

    public void SetSpeedRange(float min, float max)
    {
        minSpeed = min;
        maxSpeed = max;
    }

    public void SetAltoInput(Vector3 dir, float mag)
    {
        altoDirection = dir;
        altoMagnitude = mag;
    }

    public void UpdateMagnitude(float value)
    {
        float alphaStep = 150.0f / 255.0f;
        value *= alphaStep;
        magnitude.color = new Color(magnitude.color.r, magnitude.color.g, magnitude.color.b, value);
        directionArrow.color = magnitude.color;
    }

    public void UpdateSpeedUI(bool increase=true, bool on=true)
    {
        if (on)
        {
            if (increase) { upSpeed.sprite = arrowFilled; downSpeed.sprite = arrowEdge; }
            else { downSpeed.sprite = arrowFilled; upSpeed.sprite = arrowEdge; }
        }
        else
        {
            upSpeed.sprite = arrowEdge;
            downSpeed.sprite = arrowEdge;
        }
    }

    public void UpdateSpeed(float value)
    {
        value -= minSpeed;
        if(value > 0.0f) { value /= (maxSpeed - minSpeed); }
        speed.fillAmount = value;
    }

    public void UpdateAmmo(int currentAmmo)
    {
        float sectionValue = 0.0385f;
        AmmoUI.fillAmount = sectionValue * (float)currentAmmo;
       if(currentAmmo < 5)
        {
            if (!lowAmmoWarning)
            {
                StartCoroutine(AmmoWarning(Color.red));
            }
        }
       else
        {
            lowAmmoWarning = false;
        }
    }

    public void UpdateScore(float currentScore)
    {
        Score.text = "Score:  " + currentScore.ToString();
    }

    public void SpecialAmmo()
    {
        StartCoroutine(AmmoWarning(Color.magenta));
    }

    public void ResetSpecialAmmo()
    {
        lowAmmoWarning = false;
    }
    
    private IEnumerator AmmoWarning(Color goalColor)
    {
        lowAmmoWarning = true;
        Color baseColor = AmmoIcon.color;
        goalColor.a = baseColor.a;
        float timer = 0.0f;
        float step = 0.0f;
        bool reverse = false;
        while (lowAmmoWarning)
        {
            timer += Time.deltaTime;
            if(timer <= 0.5f)
            {
                if (reverse)
                {
                    AmmoIcon.color = Color.Lerp(goalColor, baseColor, step);
                    AmmoUI.color = AmmoIcon.color;
                }
                else
                {
                    AmmoIcon.color = Color.Lerp(baseColor, goalColor, step);
                    AmmoUI.color = AmmoIcon.color;
                }
                step = timer / 0.5f;
            }
            else
            {
                timer = 0.0f;
                step = 0.0f;
                reverse = !reverse;
            }
            yield return false;
        }
        AmmoIcon.color = baseColor;
        AmmoUI.color = baseColor;
    }

    private void UpdateDirection()
    {
        if(altoDirection != Vector3.zero && altoMagnitude >= minMagnitude)
        {
            float angle = Mathf.Atan2(altoDirection.x, altoDirection.z) * Mathf.Rad2Deg;
            Vector3 correction = transform.rotation.eulerAngles;
            float correctionY = correction.y; //allows arrow to always be facing the travel direction in the HMD
            angle += Mathf.Ceil(-angle / 360.0f) * 360.0f; //keep it within 0 to 359
            Quaternion rotatedAxis = Quaternion.Euler(60.0f, 0.0f, (angle - correctionY) * -1.0f); //gui rect rotations are inversed so have to rotate counterclockwise
            direction.localRotation = rotatedAxis;
        }
    }

    private void UpdateUpArrow()
    {
        switch (upArrowState)
        {
            case ArrowState.Hide:
                if(upArrow.fillAmount != 0.0f)
                {
                    upArrow.fillAmount -= arrowStep;
                }
                break;
            case ArrowState.Reveal:
                if (upArrow.fillAmount != 1.0f)
                {
                    upArrow.fillAmount += arrowStep;
                }
                else { upArrowState = ArrowState.Visible; }
                break;
            case ArrowState.Visible:
                break;
        }
    }

    private void UpdateDownArrow()
    {
        switch (downArrowState)
        {
            case ArrowState.Hide:
                if (downArrow.fillAmount > 0.0f)
                {
                    downArrow.fillAmount -= arrowStep;
                }
                break;
            case ArrowState.Reveal:
                if (downArrow.fillAmount < 1.0f)
                {
                    downArrow.fillAmount += arrowStep;
                }
                else { downArrowState = ArrowState.Visible; }
                break;
            case ArrowState.Visible:
                break;
        }
    }

    private void Update()
    {
        UpdateDirection();
        UpdateUpArrow();
        UpdateDownArrow();
    }
}
