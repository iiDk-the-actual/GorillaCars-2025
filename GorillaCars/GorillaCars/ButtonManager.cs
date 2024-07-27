using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GorillaCars
{
    public class ButtonManager : GorillaPressableButton
    {
        public override void Start()
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            gameObject.layer = 18;


            onPressButton = new UnityEngine.Events.UnityEvent();
            onPressButton.AddListener(new UnityEngine.Events.UnityAction(ButtonActivation));

        }

        public override void ButtonActivation()
        {
            manager manager = FindAnyObjectByType<manager>();
            manager.clicked(this.gameObject.name);
        }
    }
}
