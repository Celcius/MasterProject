using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityCollideable<T> : MonoBehaviour where T : Component
{
    [SerializeField]
    private string colliderTag = GameConstants.PLAYER_TAG;
    public virtual string ColliderTag => colliderTag;



    public virtual bool IsCollisionEntity(Collider2D other, out T entity)
    {
        return IsCollisionEntity(other, out entity, ColliderTag);
    }

    public static bool IsCollisionEntity(Collider2D other, out T entity, string tag)
    {
        if(other.tag == tag)
        {
            entity = other.GetComponent<T>();
            return entity != null;
        }
        entity = null;
        return false;
    }

    protected virtual void OnCollisionEnter2D(Collision2D other) 
    {
        T entity;
        if(IsCollisionEntity(other.collider, out entity))
        {
           EntityCollisionEnter(entity);
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D other) 
    {
        T entity;
        if(IsCollisionEntity(other.collider, out entity))
        {
            EntityCollisionExit(entity);
        }
    }

    protected void OnCollisionStay2D(Collision2D other) 
    {
        T entity;
        if(IsCollisionEntity(other.collider, out entity))
        {
           EntityCollisionStay(entity);
        }
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D other) 
    {
        T entity;
        if(IsCollisionEntity(other, out entity))
        {
           EntityTriggerEnter(entity);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        T entity;
        if(IsCollisionEntity(other, out entity))
        {
            EntityTriggerExit(entity);
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D other) 
    {
        T entity;
        if(IsCollisionEntity(other, out entity))
        {
           EntityTriggerStay(entity);
        }
    }

    protected virtual void EntityCollisionEnter(T entity) {}
    protected virtual void EntityCollisionExit(T entity) {}
    protected virtual void EntityCollisionStay(T entity) {}

    protected virtual void EntityTriggerEnter(T entity) {}
    protected virtual void EntityTriggerExit(T entity) {}
    protected virtual void EntityTriggerStay(T entity) {}
}
