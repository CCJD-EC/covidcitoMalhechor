using System;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private GameObject[] mPrefab;

    // Start is called before the first frame update
    private GameObject _spawned;

    private Animator m_animator;

    private List<GameObject> n_items = new List<GameObject>();

    private List<Vector3> n_coordinates = new List<Vector3>()
    {
        new Vector3(-0.061f, 0.95f, -0.069f),
        new Vector3(-0.27f, 0.95f, -0.152f),
        new Vector3(-0.154f, 0.95f, 0.144f),
        new Vector3(0.09f, 0.95f, 0.119f),
        new Vector3(0.082f, 0.95f, -0.126f),
        new Vector3(-0.331f, 0.95f, 0.169f),
        new Vector3(0.331f, 0.95f, 0.049f)
    };


    private float _ypositionAbs = 0;

    public GameObject[] MPrefab
    {
        get => mPrefab;
        set => mPrefab = value;
    }

    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (n_items.Count == 0 && !AnimatorIsPlaying())
        {
            var index = 0;
            List<Vector3> tempCoord = new List<Vector3>();
            var position = transform.position;
            var newPosition = new Vector3(position.x, position.y, position.z);
            foreach (Vector3 vector in n_coordinates)
            {
                var itemPosition = newPosition + vector;
                n_items.Add(Instantiate(mPrefab[index], itemPosition, mPrefab[index].transform.rotation));
                tempCoord.Add(itemPosition);
                if (_ypositionAbs == 0)
                {
                    _ypositionAbs = itemPosition.y;
                }

                if (index == 0)
                {
                    n_items[0].SetActive(false);
                }

                index++;
            }

            n_coordinates.Clear();
            n_coordinates = tempCoord;
            Debug.Log("New Array position " + n_coordinates);
        }
        else
        {
            var i = 0;
            foreach (var item in n_items)
            {
                try
                {
                    var state = item.GetComponent<Propierties>().Vstate;
                    if (state.Equals("D"))
                    {
                        Debug.Log("Objects List" + n_items.Count + "Y abs " + _ypositionAbs);
                        Destroy(n_items[i]);
                        n_items[i] = Instantiate(item, n_coordinates[i], item.transform.rotation);
                        Debug.Log("Object Deleted");
                        return;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Error, Can't get to state\n"+e);
                }

                i++;
                Debug.Log("n_coordinates[i].y " + i + " " + item.transform.position.y);
            }
        }
    }

    bool AnimatorIsPlaying()
    {
        return m_animator.GetCurrentAnimatorStateInfo(0).length >
               m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}