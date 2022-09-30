using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Coursework
{
    public class Collidable
    {
        #region Fields
        public bool flagForRemoval = false;
        protected Rectangle boundingRectangle = new Rectangle();
        public Rectangle BoundingRectangle
        {
            get { return boundingRectangle; }
        }
        #endregion

        #region Member Functions
        public virtual bool CollisionTest(Collidable obj)
        {
            return false;
        }

        public virtual void OnCollision(Collidable obj)
        {
        }
        #endregion
    }
}
