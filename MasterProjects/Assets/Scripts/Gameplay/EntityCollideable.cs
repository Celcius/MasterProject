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
           PlayerCollisionEnter(entity);
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D other) 
    {
        T entity;
        if(IsCollisionEntity(other.collider, out entity))
        {
            PlayerCollisionExit(entity);
        }
    }

    protected void OnCollisionStay2D(Collision2D other) 
    {
        T entity;
        if(IsCollisionEntity(other.collider, out entity))
        {
           PlayerCollisionStay(entity);
        }
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D other) 
    {
        T entity;
        if(IsCollisionEntity(other, out entity))
        {
           PlayerTriggerEnter(entity);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        T entity;
        if(IsCollisionEntity(other, out entity))
        {
            PlayerTriggerExit(entity);
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D other) 
    {
        T entity;
        if(IsCollisionEntity(other, out entity))
        {
           PlayerTriggerStay(entity);
        }
    }

    protected virtual void PlayerCollisionEnter(T entity) {}
    protected virtual void PlayerCollisionExit(T entity) {}
    protected virtual void PlayerCollisionStay(T entity) {}

    protected virtual void PlayerTriggerEnter(T entity) {}
    protected virtual void PlayerTriggerExit(T entity) {}
    protected virtual void PlayerTriggerStay(T entity) {}
}
