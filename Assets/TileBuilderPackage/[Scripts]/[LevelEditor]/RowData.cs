using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RowData {
	private string rowName = "";
	private bool rowActive = false;
	public List<TagData> tagData = new List<TagData>();
		
	public string RowName {
		get { return rowName;}
		set{ rowName = value;}
	}
		
	public bool RowActive {
		get { return rowActive;}
		set { rowActive = value;}
	}
		
	public List<TagData> TagDataList {
		get{ return tagData;}
		set{ tagData = value;}
	}
}

public class TagData {
	private string tagName = "";
	private bool tagActive = false;
		
	public string TagName {
		get { return tagName;}
		set{ tagName = value;}
	}
	
	public bool TagActive {
		get { return tagActive;}
		set { tagActive = value;}
	}
}