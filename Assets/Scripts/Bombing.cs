using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bombing : MonoBehaviour
{
    [Header("CDManagement")] 
    public float firingDuration = 5;
    public float reloadingDuration = 2;
    private float firingStatus;
    private float reloadingStatus;
    private ParticleSystem.EmissionModule emis;
    
    // Start is called before the first frame update
    void Start()
    {

        firingStatus = firingDuration;
        reloadingStatus = 0;
        
        emis = transform.Find("Particle System").GetComponent<ParticleSystem>().emission;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (firingStatus > 0 && reloadingStatus == 0)
        {
            firingStatus -= Time.deltaTime;
        }
        else if (firingStatus <= 0 && reloadingStatus == 0)
        {
            reloadingStatus = reloadingDuration;
            firingStatus = 0;
            emis.rateOverTime = 0.0f;
        }

        if (reloadingStatus > 0 && firingStatus == 0)
        {
            reloadingStatus -= Time.deltaTime;
        }
        else if (reloadingStatus <= 0 && firingStatus == 0)
        {
            firingStatus = firingDuration;
            reloadingStatus = 0;
            emis.rateOverTime = 10.0f;
        }
        
    }
}
