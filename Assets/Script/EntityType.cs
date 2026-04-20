using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EntityCategory
{
    Fish,
    Trash
}

public class ParsedFileData
{
    public EntityCategory category;
    public string type;
    public string fullPath;
}
public class EntityType : MonoBehaviour
{

}
