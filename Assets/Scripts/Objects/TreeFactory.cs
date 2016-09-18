using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    public static class TreeFactory
    {
        static HashSet<PraeTree> treesInScene; //contains all trees generated

        static string p = "Trees/"; // path to tree prefab folder
        static string[] treePrefabs = {
            p +"tree1_pre",
            p +"tree2_pre",
            p +"tree3_pre",
            p +"tree4_pre"
        };


        public static PraeTree makeRandomTreeCollisionFree(Vector3 atPos)
        {
            int treeid = UnityEngine.Random.Range(0, treePrefabs.Length);
            return makeTreeCollisionFree(treePrefabs[treeid], atPos);
        }

        public static PraeTree makeTreeCollisionFree(string treepath, Vector3 atPos)
        {
            throw new NotImplementedException();
        }

        /**
         * func: instantiate a tree based on prefab 'treepath' at position 0,0,0
         * @treepath:    path to tree prefab (must contain 'SpriteTree' as component)
         * @anim_offset: TRUE => start index of animation is randomized
         */
        public static PraeTree makeTree(string treepath, bool anim_offset = false)
        {
            GameObject newTreeObj = GameObject.Instantiate(Resources.Load(treepath)) as GameObject;
            PraeTree newTree = newTreeObj.GetComponent<PraeTree>();
            if (newTree == null)
                throw new NullReferenceException("makeTree: gameobject does not have SpriteTree component!");
            
            if (anim_offset)
            {
                Animator anim = newTreeObj.GetComponent<Animator>();
                AnimatorStateInfo stateinfo = anim.GetCurrentAnimatorStateInfo(0);
                if (stateinfo.IsName("Idle"))
                    anim.SetFloat("animoffset", UnityEngine.Random.Range(0, stateinfo.length));
                else
                    Debug.Log("makeTree: Wrong state for anim offset!");
            }

            if (treesInScene == null)
                treesInScene = new HashSet<PraeTree>();
            treesInScene.Add(newTree);

            return newTree;
        }

        /**
         * func: instantiate a tree based on prefab 'treepath' at position 'atPos'
         * @treepath: path to tree prefab (must contain 'SpriteTree' as component)
         * @atPos: position of the newly generated tree
         */
        public static PraeTree makeTree(string treepath, Vector3 atPos, bool anim_offset = false)
        {
            PraeTree newTree = makeTree(treepath, anim_offset);
            newTree.gameObject.transform.position = atPos;
            return newTree;
        }

        /**
         * func: instantiate a randomly chosen tree at position 0,0,0
         */
        public static PraeTree makeRandomTree()
        {
            int treeid = UnityEngine.Random.Range(0, treePrefabs.Length);
            return makeTree(treePrefabs[treeid], Vector3.zero);
        }

        /**
         * func: instantiate a randomly chosen tree at position 'atPos'
         * @atPos: the position of the transform of the resulting tree
         */
        public static PraeTree makeRandomTreeAtPos(Vector3 atPos, bool anim_offset = false)
        {
            int treeid = UnityEngine.Random.Range(0, treePrefabs.Length);
            return makeTree(treePrefabs[treeid], atPos, anim_offset);
        }
    }
}
