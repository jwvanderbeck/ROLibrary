﻿using System;
using UnityEngine;

namespace ROLib
{

    /// <summary>
    /// Utility class for methods that manipulate part attach nodes.
    /// </summary>
    public static class ROLAttachNodeUtils
    {

        /// <summary>
        /// Updates an attach node position and handles offseting of any attached parts (or base part if attached part is the parent). <para/>
        /// Intended to replace the current per-part-module code that does the same, with a centrally managed utility method, for convenience and easier bug tracking and fixing.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="node"></param>
        /// <param name="newPos"></param>
        /// <param name="orientation"></param>
        public static void updateAttachNodePosition(Part part, AttachNode node, Vector3 newPos, Vector3 orientation, bool updatePartPosition, int size)
        {
            Vector3 diff = newPos - node.position;
            node.position = node.originalPosition = newPos;
            node.orientation = node.originalOrientation = orientation;
            node.size = size;
            if (updatePartPosition && node.attachedPart != null)
            {
                Vector3 globalDiff = part.transform.TransformPoint(diff);
                globalDiff -= part.transform.position;
                if (node.attachedPart.parent == part)//is a child of this part, move it the entire offset distance
                {
                    node.attachedPart.attPos0 += diff;
                    node.attachedPart.transform.position += globalDiff;
                }
                else//is a parent of this part, do not move it, instead move this part the full amount
                {
                    part.attPos0 -= diff;
                    part.transform.position -= globalDiff;
                    //and then, if this is not the root part, offset the root part in the negative of the difference to maintain relative part position
                    Part p = part.localRoot;
                    if (p != null && p != part)
                    {
                        p.transform.position += globalDiff;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new attach node with the given paramaters and adds it to the input part.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        /// <param name="orient"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static AttachNode createAttachNode(Part part, String id, Vector3 pos, Vector3 orient, int size)
        {
            AttachNode newNode = new AttachNode();
            newNode.id = id;
            newNode.owner = part;
            newNode.nodeType = AttachNode.NodeType.Stack;
            newNode.size = size;
            newNode.originalPosition = newNode.position = pos;
            newNode.originalOrientation = newNode.orientation = orient;
            part.attachNodes.Add(newNode);
            return newNode;
        }

        /// <summary>
        /// Destroys the input attach node and removes it from the part.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="node"></param>
        public static void destroyAttachNode(Part part, AttachNode node)
        {
            if (node == null) { return; }
            if (node.attachedPart != null) { MonoBehaviour.print("ERROR: Deleting attach node: " + node.id + " with attached part: " + node.attachedPart); }
            part.attachNodes.Remove(node);
            node.owner = null;
            if (node.icon != null)
            {
                GameObject.Destroy(node.icon);
            }
        }

        //TODO clean this up to handle children that are attached to top or bottom faces rather than sides.  Will need to include inputs for heights.
        /// <summary>
        /// Updates surface attached children of the input part.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="oldDiameter"></param>
        /// <param name="newDiameter"></param>
        public static void updateSurfaceAttachedChildren(Part part, float oldDiameter, float newDiameter)
        {
            float delta = (newDiameter - oldDiameter) / 2;
            Vector3 parentLS = part.transform.localPosition;
            foreach (Part child in part.children)
            {
                if (child.srfAttachNode is AttachNode n && n.attachedPart == part)//has surface attach node, and surface attach node is attached to the input part
                {
                    // The child must displace radially in the coordinate system of the parent
                    // Work in the parent coordinate space, then translate in world.
                    Vector3 childInParentSpace = part.transform.InverseTransformPoint(child.transform.position);
                    Vector3 dir = childInParentSpace - parentLS;
                    dir.y = 0;
                    dir.Normalize();
                    // Debug.Log($"[ROLAttachNode] Moving surface-attached children by {delta} in dir {dir} in parent space");
                    Vector3 dir_w = part.transform.TransformDirection(dir);
                    child.transform.Translate(dir_w * delta, Space.World);
                    child.attPos0 = child.transform.localPosition;
                }
            }
        }
    }

    /// <summary>
    /// Persistent static data for an attach node position for a ModelBaseData.
    /// These instances will be copied into the live model data class into an AttachNodeData instance,
    /// which includes utility methods for updating the node position and has non-readonly fields for run-time manipulation.
    /// </summary>
    public class AttachNodeBaseData
    {
        public readonly Vector3 position;
        public readonly Vector3 orientation;
        public readonly int size;

        public AttachNodeBaseData(String nodeData)
        {
            String[] dataVals = nodeData.Split(new String[] { "," }, StringSplitOptions.None);
            position = new Vector3(ROLUtils.safeParseFloat(dataVals[0].Trim()), ROLUtils.safeParseFloat(dataVals[1].Trim()), ROLUtils.safeParseFloat(dataVals[2].Trim()));
            orientation = new Vector3(ROLUtils.safeParseFloat(dataVals[3].Trim()), ROLUtils.safeParseFloat(dataVals[4].Trim()), ROLUtils.safeParseFloat(dataVals[5].Trim()));
            size = dataVals.Length > 6 ? ROLUtils.safeParseInt(dataVals[6]) : 4;
        }

        public AttachNodeBaseData(float x, float y, float z, float rx, float ry, float rz, float size)
        {
            position = new Vector3(x, y, z);
            orientation = new Vector3(rx, ry, rz);
            this.size = Mathf.RoundToInt(size);
        }
    }
}
