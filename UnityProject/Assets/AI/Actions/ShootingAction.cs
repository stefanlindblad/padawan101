using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class ShootingAction : RAINAction
{
    //Transform m_cannonRot;
    Transform m_muzzle;
    GameObject m_shotPrefab;
    Vector3 rndAngle;
    float spread = 0.0f; //5.0

    public override void Start(RAIN.Core.AI ai)
    {
        //m_cannonRot = ai.Body.GetComponent<CannonBehavior>().m_cannonRot;
        m_muzzle = ai.Body.GetComponent<CannonBehavior>().m_muzzle;
        m_shotPrefab = ai.Body.GetComponent<CannonBehavior>().m_shotPrefab;

        base.Start(ai);
    }

    public RandomFromDistribution.ConfidenceLevel_e conf_level = RandomFromDistribution.ConfidenceLevel_e._95;

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
        // Make the shot distorted (not so perfect)
        rndAngle.x = RandomFromDistribution.RandomRangeNormalDistribution(-spread, spread, conf_level);
        rndAngle.y = RandomFromDistribution.RandomRangeNormalDistribution(-spread, spread, conf_level);

        Quaternion distortion = Quaternion.Euler(rndAngle.x, rndAngle.y, 0);
        Quaternion direction = m_muzzle.rotation * distortion;

        // Shoot a ray that disipears after 5 sec
        //GameObject go = GameObject.Instantiate(m_shotPrefab, m_muzzle.position, direction) as GameObject;
        // GameObject.Destroy(go, 5f);
        var playerObj = GameObject.FindWithTag("Player");
        var player = playerObj.GetComponent<PlayerManager>();
        player.CmdSpawnShot(m_muzzle.position, direction);
        return ActionResult.SUCCESS;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}