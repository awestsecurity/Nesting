using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EventPeriod : MonoBehaviour
{
	
	public DateTime startDate;
	public DateTime endDate;
	public int templateID; // Which bird is affected
	public int AddTimeAllowed = 0; // How much time is added
	public int changeSeedTo = 0; // Alter play area so specified seed
	
    public bool IsActive() {
		DateTime today = DateTime.Now;
		return ( today > startDate && today < endDate ) ? true : false;
	}
	
}
