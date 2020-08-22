using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private float startPosX;
    private float startPosY;
    private int modifier = 1;
    private bool isHeld = false;
    public CharacterCreation Main;
    private void Update()
    {
        if (isHeld)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gameObject.transform.localPosition = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY, -1);
        }
    }
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)) // move
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            startPosX = mousePos.x - transform.localPosition.x;
            startPosY = mousePos.y - transform.localPosition.y;
            isHeld = true;
        }
        else if (Input.GetMouseButtonDown(1)) // flip
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, -1);
            modifier *= -1;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // make bigger
        {
            transform.localScale = new Vector3(transform.localScale.x + 0.05f * modifier, transform.localScale.y + 0.05f, -1);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // make smaller
        {
            if(transform.localScale.y > 0.08f)
            transform.localScale = new Vector3(transform.localScale.x - 0.05f * modifier, transform.localScale.y - 0.05f, -1);
        }
    }
    private void OnMouseUp()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.x < -7.8 && mousePos.y > 3.8)
        {
            Main.FillLists();
            Main.allspawned.Remove(gameObject.name);
            Destroy(gameObject);
        }
        isHeld = false;
    }
}

