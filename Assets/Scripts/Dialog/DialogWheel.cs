using NUnit.Framework;
using System;
using System.Linq;
using TMPro;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static System.Net.Mime.MediaTypeNames;

public class DialogWheel : MonoBehaviour
{
    public Dialog dialog;
    Vector2 Location = Vector2.zero;
    InputAction open;
    Mouse mouse;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mouse = Mouse.current;
        open = InputSystem.actions.FindAction("Dialog");
    }
    bool visable = false;
    void ToggleDraw()
    {
        visable = !visable;
        Renderer[] renderers = this.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = visable;
        }

        Canvas[] canvases = this.GetComponentsInChildren<Canvas>(true);
        foreach (Canvas canvas in canvases)
        {
            canvas.enabled = visable;
        }
    }

    //got  to be a better way to get this
    const float radi =15f;
    const float innerRadi = 0.5f;
    // Update is called once per frame
    void Update()
    {
        if (open.IsPressed())
        {

            if (Location == Vector2.zero)
            {

                dialog = new Dialog();
                //Todo populate properly by searching nearby objects
                PopulateDialog();
                Location = new Vector3(mouse.position.x.ReadValue(), mouse.position.y.ReadValue(), UnityEngine.Camera.main.nearClipPlane);
                var actPos = UnityEngine.Camera.main.ScreenToWorldPoint(Location);

                actPos.z = 0;

                transform.position = actPos;
                var RotationPerOption = (Mathf.PI * 2) / dialog.options.Count;
                for (int i = 0; i < dialog.options.Count; i++)
                {
                    //Line
                    var obj = new GameObject($"UiLine{i}");
                    obj.transform.parent = this.transform;
                    var lr = obj.AddComponent<LineRenderer>();
                    lr.sortingLayerName = "UI";
                    Vector3[] pos = new Vector3[2];
                    pos[0] = this.transform.position;
                    Vector2 direction = new Vector2(1, 0).RotateBy(i * RotationPerOption);
                    Vector3 outer = new Vector3(direction.x, direction.y, 0) * radi;
                    pos[1] = pos[0] + outer;
                    lr.SetPositions(pos);
                    lr.SetWidth(0.1f, 0.1f);
                    lr.material = new Material(Shader.Find("Sprites/Default"));
                    lr.startColor = Color.red;
                    lr.endColor = Color.red;



                    //Text
                    //doesnt really work and has gotten crazy bloatted from failed rewrite attempts. Probably best to rewrite this soon
                    Vector2 RotatedVector = new Vector2(1, 0).RotateBy(i * RotationPerOption + (RotationPerOption / 2));
                    var parent = new GameObject($"UiTextParent{i}");
                    var c = parent.AddComponent<Canvas>();
                    c.sortingLayerName = "UI";
                    var rect = parent.GetComponent<RectTransform>();
                    rect.sizeDelta = new Vector2(200, 100);
                    parent.transform.SetParent(this.transform);
                    rect.position += new Vector3(RotatedVector.x, RotatedVector.y, 0) * radi / 2;
                    var text = new GameObject($"UiText{i}");
                    text.transform.SetParent(parent.transform);
                    var txt = text.AddComponent<TextMeshProUGUI>();
                    var txtRect = text.GetComponent<RectTransform>();
                    txtRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400);
                    txtRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 2000);
                   
                    txt.extraPadding = true;
                    txt.text = $"" + dialog.options.Keys.ElementAt(i);
                    //Higher font size + scaling down creates less blurry text
                    txt.fontSize = 40;
                    txt.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    txt.transform.position = this.transform.position + new Vector3(parent.transform.position.x, parent.transform.position.y, 0);
                    txt.textWrappingMode = TextWrappingModes.PreserveWhitespace;
                    txt.alignment = TextAlignmentOptions.MidlineLeft;


                }
                ToggleDraw();

            }

        }
        else
        {
            if (Location != Vector2.zero)
            {
                var mousePos = new Vector3(mouse.position.x.ReadValue(), mouse.position.y.ReadValue(), 0);
                var actPos = UnityEngine.Camera.main.ScreenToWorldPoint(mousePos);
                float dist = Vector2.Distance(actPos, transform.position);
                if (dist > innerRadi)
                {
                    float angle = Mathf.Atan2(actPos.y - transform.position.y, actPos.x - transform.position.x);
                    angle = (angle + Mathf.PI * 2) % (Mathf.PI * 2);
                    int index = (int)(angle / (Mathf.PI * 2 / dialog.options.Count));
                    dialog.options.Values.ElementAt(index).Invoke();
                }
                ToggleDraw();
                CleanUpLines();
                Location = Vector2.zero;
            }


        }
    }

    private void PopulateDialog()
    {
        Controllable player = Controllable.Current;
        if (player != null)
        {
            foreach (var obj in Physics2D.OverlapCircleAll(player.transform.position, player.DialogSearchRadius))
            {
                var ds = obj.GetComponent<IDialogSource>();
                if (ds != null)
                {

                    Debug.Log("passed");
                    ds.ModifyDialog(dialog, ds, player);
                }
            }
        }

    }

    private void CleanUpLines()
    {
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Ui"))
            {
                Destroy(child.gameObject);
            }
        }
    }
}
