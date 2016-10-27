using Assets.Scripts.Entity;

namespace Assets.Scripts.Buffs
{
    [System.Serializable]
    public abstract class AbstractBuff
    {
        public int id;
        protected float _duration;
        public float restDuration;
        public bool bDurationEffect;
        protected EntityInfo _buffedEntity;


        public AbstractBuff() {}

        public AbstractBuff(EntityInfo buffedEntity)
        {
            this.buffedEntity = buffedEntity;
        }


        public bool hasDurationEffect() { return bDurationEffect; }

        /**
         * is initially called from the constructor
         */
        protected abstract void applyStartEffect();

        public abstract void applyEndEffect();

        /**
         * func: apply a time dependend effect on ei
         * @ei receiver of the effect
         * @dTime: the time past since the last call
         */
        public abstract void applyDurationEffect(float dTime);


        /**
         * GETTER AND SETTER
         *********************/

        public EntityInfo buffedEntity
        {
            get { return _buffedEntity; }
            set
            {
                if (value == null)
                    throw new System.NullReferenceException("buffed entity must never be set to NULL!");
                else
                {
                    _buffedEntity = value;
                    applyStartEffect();
                }
            }
        }

        public float duration
        {
            get { return _duration; }
            set
            {
                if (value < 0)
                    UnityEngine.Debug.LogError("duration cannot be negative!");
                _duration = value;
            }
        }
    }
}
