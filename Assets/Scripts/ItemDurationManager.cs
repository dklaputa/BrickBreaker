using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemDurationManager : MonoBehaviour
{
    public static ItemDurationManager instance;
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
    }

    // Update is called once per frame
    private void Update()
    {
        if (needMove.Count < 1) return;
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
                    position, Time.deltaTime * 2 * distance);
            else
            {
                needMove.Remove(i);
            }
        }
    }

    public void ShowItemDuration(int item)
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

    public void Remove(int item)
    {
        var index = list.IndexOf(item);
        for (var i = index + 1; i < list.Count; i++)
        {
            needMove.Add(list[i]);
        }
        list.RemoveAt(index);
    }
}