using UnityEngine;

public class TestNPC : Controllable, IDialogSource
{
    
    public void ModifyDialog(Dialog dialog, IDialogSource you, Controllable player)
    {

        string msg = "I HATE TRIANGLES I HATE TRIANGLES I HATE TRIANGLES I HATE TRIANGLES I HATE TRIANGLES I HATE TRIANGLES     " + Random.Range(0, 25);
        dialog.AddOption(msg, () => {
            Debug.Log(":" + msg  );
            this.Suspicion += 1;
        });
    }
    float timer = 0;
    public override void UncontrolledUpdate()
    {
        timer += Time.deltaTime;
        if(timer > 0.3 )
        {
            GetComponent<Rigidbody2D>().linearVelocity = new Vector2(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
            timer = 0;
        }
    }
}
