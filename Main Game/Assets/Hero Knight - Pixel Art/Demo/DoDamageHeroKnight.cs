using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamageHeroKnight : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D other) {
		if (other.GetComponent<EvilWizard>()) {
			other.GetComponent<EvilWizard>().health -= GetComponentInParent<HeroKnight>().damage;
			other.GetComponent<EvilWizard>().m_animator.SetTrigger("Hurt");			
		}
	}
}