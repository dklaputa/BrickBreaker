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
    public GameObject[] itemDuration;
    public GameObject[] itemButtons;

    private ItemDurationScript[] itemDurationScripts;
    private const float itemDurationDistance = 90 / 192f;
    private List<int> itemDurationList;
    private HashSet<int> needMove;
    private bool isMoving;

    private Text[] itemNumTexts;
    private Animator[] itemNumAnimators;
    private int[] itemNum;
    private Vector3 itemDurationOriginPosition;

    // Use this for initialization
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        itemDurationList = new List<int>();
        needMove = new HashSet<int>();
        itemDurationScripts = new ItemDurationScript[2];
        itemDurationScripts[0] = itemDuration[0].GetComponent<ItemDurationScript>();
        itemDurationScripts[0].GetComponent<ItemDurationScript>().Initialize(0, 10);
        itemDurationScripts[1] = itemDuration[1].GetComponent<ItemDurationScript>();
        itemDurationScripts[1].GetComponent<ItemDurationScript>().Initialize(1, 10);
        itemDurationOriginPosition = itemDuration[0].transform.position;

        itemNum = new[] {1, 1};
        itemNumTexts = new[]
        {
            itemButtons[0].transform.GetChild(0).GetChild(0).GetComponent<Text>(),
            itemButtons[1].transform.GetChild(0).GetChild(0).GetComponent<Text>()
        };
        itemNumAnimators = new[]
        {
            itemButtons[0].transform.GetChild(0).GetComponent<Animator>(),
            itemButtons[1].transform.GetChild(0).GetComponent<Animator>()
        };
    }

    // Update is called once per frame
    private IEnumerator Move()
    {
        isMoving = true;
        for (;;)
        {
            if (needMove.Count > 0)
            {
                var l = needMove.ToList();
                foreach (var i in l)
                {
                    if (!itemDurationList.Contains(i))
                    {
                        needMove.Remove(i);
                        continue;
                    }
                    var position = itemDurationOriginPosition +
                                   Vector3.right * itemDurationDistance * itemDurationList.IndexOf(i);
                    if (itemDurationScripts[i].transform.position != position)
                        itemDurationScripts[i].transform.position = Vector2.MoveTowards(
                            itemDurationScripts[i].transform.position,
                            position, 2 * Time.deltaTime * itemDurationDistance);
                    else
                    {
                        needMove.Remove(i);
                    }
                }
                yield return null;
            }
            else break;
        }
        isMoving = false;
    }

    public int CheckItemLevel(int item)
    {
        return !itemDurationScripts[item].gameObject.activeInHierarchy ? 0 : itemDurationScripts[item].GetLevel();
    }

    public void ObtainItem()
    {
        //Need improve.
        if (Random.value < .5f)
        {
            itemNum[0]++;
            itemNumTexts[0].text = itemNum[0].ToString();
            itemNumAnimators[0].SetTrigger("Shake");
        }
        else
        {
            itemNum[1]++;
            itemNumTexts[1].text = itemNum[1].ToString();
            itemNumAnimators[1].SetTrigger("Shake");
        }
    }

    public void OnItemClick(int item)
    {
        if (itemNum[item] < 1 || !GameController.instance.IsGameStart()) return;
        itemNum[item]--;
        itemNumTexts[item].text = itemNum[item].ToString();
        if (itemDurationList.Contains(item)) itemDurationScripts[item].LevelUp();
        else
        {
            if (itemDurationList.Count > 0)
                itemDurationScripts[item].transform.position =
                    itemDurationScripts[itemDurationList[itemDurationList.Count - 1]].transform.position +
                    Vector3.right * itemDurationDistance;
            else itemDurationScripts[item].transform.position = itemDurationOriginPosition;

            itemDurationScripts[item].gameObject.SetActive(true);
            itemDurationList.Add(item);
            needMove.Add(item);
        }
    }

    public void OnItemDurationEnd(int item)
    {
        var index = itemDurationList.IndexOf(item);
        for (var i = index + 1; i < itemDurationList.Count; i++)
        {
            needMove.Add(itemDurationList[i]);
        }
        itemDurationList.RemoveAt(index);
        if (!isMoving) StartCoroutine("Move");
    }
}