using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SajberSim.SaveSystem
{
    public class EmptyCard : MonoBehaviour
    {
        public void CreateSave()
        {
            GameObject.Find("GameManagerObj").GetComponent<GameManager>().Save(-1);
        }
    }
}
