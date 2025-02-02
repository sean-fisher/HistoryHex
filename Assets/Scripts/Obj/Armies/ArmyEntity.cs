﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyEntity : MonoBehaviour
{
	#region Properties
	// Internal variables
	private bool activated = false;

	public int[] Position; //Position on the hex grid.
	public float Food;
	public string Name { get; set; }
	public Player Controller { get; set; }

	// UI_COMponents.
	public GameObject UIComponent; // UIComponentPrefab
	private GameObject UIComponentInstance;

	// SelectionInterface
	private SelectableObj SelectionInterface;
	public EntityDrawer drawer;
	#endregion

	// Start is called before the first frame update
	void Start()
    {
		Initialize();
	}

    // Update is called once per frame
    void Update()
    {
		// Move the responsibility of setting Unit Viewing modes to another class later.
		// Shows the Controller Color.
		if (Input.GetKeyDown(KeyCode.G)) {
			if (Controller != null) {
				drawer.Color = Controller.Colour;
			} else {
				drawer.Color = Color.black;
			}
		}
		// Clears the map.
		else if (Input.GetKeyDown(KeyCode.R)) {
			drawer.Color = Color.white;
		}

		//If Activated, run the extended activation methods.
		bool SelectedByController = Global.ActivePlayerId == Controller.PlayerId;
		if (activated && SelectedByController) {
			ActiveUpdate();
		}

		//Draw the Entity.
		Draw();
	}

	void Initialize(){
		Name = "UnnamedArmy";
		Food = Mathf.Floor(Random.value * Global.MAXIMUM_FOOD);

		// Create a drawer.
		drawer = new EntityDrawer(transform);

		//Attempt to wire the SelectionInterface.
		SelectionInterface = transform.GetComponent<SelectableObj>();
		if (SelectionInterface == null){
			throw new UnityException("Failed to link Army Entity to a SelectionInterface.");
		} else {
			WireSelectionInterface();
		}

		//Present UI Components.
	}

	private void ActiveUpdate() {
		
		// When active, listen for 7 4 1 and 9 6 3.
		if (Input.GetKeyDown(KeyCode.Keypad3)){
			int[] direction = new int[] {0, -1, 1};
			MoveAction(direction);
		} else if (Input.GetKeyDown(KeyCode.Keypad1)){
			int[] direction = new int[] { -1, 0, 1 };
			MoveAction(direction);
		} else if (Input.GetKeyDown(KeyCode.Keypad4)){
			int[] direction = new int[] { -1, 1, 0 };
			MoveAction(direction);
		} else if (Input.GetKeyDown(KeyCode.Keypad6)) {
			int[] direction = new int[] { 1, -1, 0 };
			MoveAction(direction);
		} else if (Input.GetKeyDown(KeyCode.Keypad9)) {
			int[] direction = new int[] { 1, 0, -1 };
			MoveAction(direction);
		} else if (Input.GetKeyDown(KeyCode.Keypad7)) {
			int[] direction = new int[] { 0, 1, -1 };
			MoveAction(direction);
		}

		return;
	}

	#region Unit Actions

	/// <summary>
	/// Moves the unit across the board relative to current position.
	/// </summary>
	public void MoveAction(int[] direction){
		Vector3 moveTo = Global.GetCubicVector(direction[0], direction[1], direction[2]);
		int[] nextPos = new int[] { Position[0] + direction[0], Position[1] + direction[1], Position[2] + direction[2] };
		if (Global.MapFlyWeight.HasHexAtCubic(nextPos)) {
			//Get the tile for any operations that might be necessary.
			GameObject HexTile = Global.MapFlyWeight.hexMap[nextPos];
			Global.MapFlyWeight.hexMap[Position].GetComponent<HexEntity>().army = null;
			Sieze(ref HexTile);
			transform.Translate(moveTo);
			Position = nextPos;
		}
	}

	public void Sieze(ref GameObject hexTile) {
		hexTile.GetComponent<HexEntity>().Controller = this.Controller;
		if(hexTile.GetComponent<HexEntity>().army != null){
			Destroy(hexTile.GetComponent<HexEntity>().army);
		}
		hexTile.GetComponent<HexEntity>().army = gameObject;
	}

	public void Die(){
		Destroy(this); //destroy the GameObj. Should handle the events alright. Check to be sure in the future.
	}
	/// <summary>
	/// Combats another unit.
	/// </summary>
	public void Combat(){
		// Not yet implemented!
		throw new UnityException("");
	}
	#endregion

	#region WireSelectionInterface
	private void WireSelectionInterface() {
		SelectionInterface.Prepare();
		SelectionInterface.OnSelect += OnSelect;
		SelectionInterface.OnDeselect += OnDeselect;
	}

	private void OnSelect() {
		activated = true;
	}

	private void OnDeselect() {
		activated = false;
	}

	#endregion

	//Draw Delegation
	private void Draw() {
		drawer.Update();
	}
}
