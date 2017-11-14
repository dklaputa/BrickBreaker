using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public const int Division = 0;
    public const int BlackHole = 1;

    public static ItemManager instance;
    public GameObject prefab;
    public Sprite[] sprites;

    private ItemDurationScript[] itemScripts;
    private const int distance = 90;
    private List<int> list;
    private HashSet<int> needMove;

    // Use this for initialization
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        list = new List<int>();
        needMove = new HashSet<int>();
        itemScripts = new ItemDurationScript[2];
        itemScripts[0] = Instantiate(prefab, transform).GetComponent<ItemDurationScript>();
        itemScripts[0].GetComponent<Image>().sprite = sprites[0];
        itemScripts[0].GetComponent<ItemDurationScript>().Initialize(0, 10);
        itemScripts[1] = Instantiate(prefab, transform).GetComponent<ItemDurationScript>();
        itemScripts[1].GetComponent<Image>().sprite = sprites[1];
        itemScripts[1].GetComponent<ItemDurationScript>().Initialize(1, 10);
        StartCoroutine("Move");
    }

    // Update is called once per frame
    private IEnumerator Move()
    {
        for (;;)
        {
            if (needMove.Count > 0)
            {
                var l = needMove.ToList();
                foreach (var i in l)
                {
                    if (!list.Contains(i))
                    {
                        needMove.Remove(i);
                        continue;
                    }
                    var position = transform.position + Vector3.right * distance * list.IndexOf(i);
                    if (itemScripts[i].transform.position != position)
                        itemScripts[i].transform.position = Vector2.MoveTowards(itemScripts[i].transform.position,
                            position, .06f * distance);
                    else
                    {
                        needMove.Remove(i);
                    }
                }
            }
            yield return new WaitForSeconds(.03f);
        }
    }

    public int CheckItem(int item)
    {
        return !itemScripts[item].gameObject.activeInHierarchy ? 0 : itemScripts[item].GetStack();
    }

    public void OnItemClick(int item)
    {
        if (list.Contains(item)) itemScripts[item].AddStack();
        else
        {
            if (list.Count > 0)
                itemScripts[item].transform.position =
                    itemScripts[list[list.Count - 1]].transform.position + Vector3.right * distance;
            else itemScripts[item].transform.position = transform.position;
            itemScripts[item].gameObject.SetActive(true);
            list.Add(item);
            needMove.Add(item);
        }
    }

    public void OnItemInvalid(int item)
    {
        var index = list.IndexOf(item);
        for (var i = index + 1; i < list.Count; i++)
        {
            needMove.Add(list[i]);
        }
        list.RemoveAt(index);
    }
}