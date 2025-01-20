using UnityEngine;

public class TestNPC : Controllable, IDialogSource
{


    public void ModifyDialog(Dialog dialog, IDialogSource you, Controllable player)
    {
        string msg = "Option" + Random.Range(0, 25);
        dialog.AddOption(msg, () => {
            Debug.Log(":" + msg  );
        });
    }

    public override void UncontrolledUpdate()
    {
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
    }
}
