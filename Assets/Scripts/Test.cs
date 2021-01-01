using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject m_prefab;

    // Start is called before the first frame update
    private GameObject spawned;

    private Animator m_animator;
    
    private List<GameObject> n_items = new List<GameObject>();

    private List<Vector3> n_coordinates = new List<Vector3>()
    {
        new Vector3(-0.20f, 1.066f, -0.106f),
        new Vector3(-0.20f, 1.066f, 0.051f),
        new Vector3(-0.20f, 1.066f, 0.17f),
        new Vector3(-0.04f, 1.066f, 0.04f),
        new Vector3(0.14f, 1.066f, 0.08f),
    };

    private float YpositionAbs = 0;
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
            List<Vector3> temp_coord = new List<Vector3>();
            var newPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            foreach (Vector3 vector in n_coordinates)
            {
                var itemPosition = newPosition + vector;
                n_items.Add(Instantiate(m_prefab, itemPosition, transform.rotation));
                temp_coord.Add(itemPosition);
                if (YpositionAbs == 0)
                {
                    YpositionAbs = itemPosition.y;
                }
            
                index++;
            }
            
            n_coordinates.Clear();
            n_coordinates = temp_coord;
            Debug.Log("New Array position " + n_coordinates);
        }
        else
        {
            var i = 0;
            foreach (var item in n_items)
            {
                Debug.Log("Objects List" + n_items.Count + "Y abs " + YpositionAbs);
                if (item.transform.position.y < 1.066f || item.transform.position.y > YpositionAbs)
                {
                    Destroy(n_items[i]);
                    n_items[i] = Instantiate(item, n_coordinates[i], transform.rotation);
                    Debug.Log("Object Deleted");
                    Debug.Log("Objects List" + n_items.Count);
                    return;
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