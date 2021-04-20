using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

public class FootprintsGenerator : MonoBehaviour
{
    [SerializeField] private GameObject footprintPrefab;
    [SerializeField] private JoystickControl joystick;
    private float generateTime;

    private List<GameObject> availableFootprintsList = new List<GameObject>();
    private List<GameObject> hiddenFootprintsList = new List<GameObject>();

    private void Start()
    {
        //for (int i = 0; i < 3; i++)
        //{
        //    GenerateFootprint();
        //}
    }

    private void Update()
    {
        if (joystick.joystickVec.y != 0)
        {
            generateTime += Time.deltaTime;
        }

        if (generateTime < 0.5 || joystick.joystickVec.y == 0) return;

        if (hiddenFootprintsList.Count > 1)
        {
            ShowFootprint();
        }
        else
        {
            GenerateFootprint();
        }
        generateTime = 0;
    }

    private void ShowFootprint()
    {
        availableFootprintsList.Add(hiddenFootprintsList.ElementAt(0));
        hiddenFootprintsList.RemoveAt(0);
        //Rotation of player
        RotateFootPrint();
        //Position of player
        availableFootprintsList.Last().transform.position = this.transform.position;
        SpriteRenderer footprintSpriteRenderer = availableFootprintsList.Last().GetComponent<SpriteRenderer>();
        footprintSpriteRenderer.color = new Color(footprintSpriteRenderer.color.r, footprintSpriteRenderer.color.g, footprintSpriteRenderer.color.b, 0);
        StartCoroutine(FadeinFootprint(footprintSpriteRenderer));
    }

    private void GenerateFootprint()
    {
        GameObject footprintObj = Instantiate(footprintPrefab, new Vector2(this.transform.position.x, this.transform.position.y), Quaternion.identity);
        availableFootprintsList.Add(footprintObj);
        RotateFootPrint();
        SpriteRenderer footprintSpriteRenderer = footprintObj.GetComponent<SpriteRenderer>();
        StartCoroutine(FadeinFootprint(footprintSpriteRenderer));
    }

    private void HideFootprint()
    {
        hiddenFootprintsList.Add(availableFootprintsList.ElementAt(0));
        availableFootprintsList.RemoveAt(0);
        SpriteRenderer footprintSpriteRenderer = hiddenFootprintsList.ElementAt(hiddenFootprintsList.Count - 1).GetComponent<SpriteRenderer>();
        StartCoroutine(FadeoutFootprint(footprintSpriteRenderer));
    }

    private void RotateFootPrint()
    {
        float angle = Mathf.Atan2(joystick.joystickVec.y, joystick.joystickVec.x) * Mathf.Rad2Deg - 90;
        Quaternion footprintRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        availableFootprintsList.ElementAt(availableFootprintsList.Count - 1).transform.rotation = footprintRotation;
    }

    private IEnumerator FadeoutFootprint(SpriteRenderer footprintSprite)
    {
        float duration = 0.3f;
        float elapsedTime = 0;
        float startValue = footprintSprite.material.color.a;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, 0, elapsedTime / duration);
            footprintSprite.color = new Color(footprintSprite.color.r, footprintSprite.color.g, footprintSprite.color.b, newAlpha);
            yield return null;
        }
    }

    private IEnumerator FadeinFootprint(SpriteRenderer footprintSprite)
    {
        float duration = 0.3f;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            footprintSprite.color = new Color(footprintSprite.color.r, footprintSprite.color.g, footprintSprite.color.b, newAlpha);
            yield return null;
        }

        yield return new WaitForSeconds(1); // 1 second to start to fade
        HideFootprint();
    }
}