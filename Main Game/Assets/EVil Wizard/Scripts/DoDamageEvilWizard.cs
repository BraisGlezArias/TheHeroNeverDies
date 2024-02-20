using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamageEvilWizard : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D other) {
		if (other.GetComponent<HeroKnight>() && !other.GetComponent<HeroKnight>().blocking && !other.GetComponent<HeroKnight>().beingHitted && other.GetComponent<HeroKnight>().health > 0) {
			other.GetComponent<HeroKnight>().health -= GetComponentInParent<EvilWizard>().damage - other.GetComponent<HeroKnight>().defense;
			other.GetComponent<HeroKnight>().m_animator.SetBool("Hurt", true);
			other.GetComponent<HeroKnight>().beingHitted = true;
			other.GetComponent<HeroKnight>().m_facingDirection = 0;
			if (GetComponentInParent<EvilWizard>().fireExists) {
				other.GetComponent<HeroKnight>().recover = 0.75f - GetComponentInParent<EvilWizard>().m_timeSinceAttack;
			} else if (GetComponentInParent<EvilWizard>().lightningExists) {
				other.GetComponent<HeroKnight>().recover = 0.6f - GetComponentInParent<EvilWizard>().m_timeSinceAttack;
			}
		}
	}

}