// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// An input adapter that translates UnityUI input events
    /// into XRI-compatible interactions.
    /// </summary>
    [AddComponentMenu("MRTK/UX/UGUI Input Adapter")]
    public class UGUIInputAdapter : Selectable, ISubmitHandler
    {
        private IXRInteractable thisInteractable;

        /// <summary>
        /// The associated <see cref="XRBaseInteractable"/> on behalf of which
        /// this adapter will translate input events.
        /// </summary>
        protected IXRInteractable ThisInteractable
        {
            get
            {
                if (thisInteractable == null)
                {
                    thisInteractable = GetComponentInParent<IXRInteractable>();
                }
                return thisInteractable;
            }
        }

        [SerializeField]
        [EnumFlags]
        [Tooltip("Which axes should be used for manipulation instead of navigation?")]
        private AxisFlags movableAxes = 0;

        /// <summary>
        /// Which axes should be used for manipulation instead of navigation? For sliders, this
        /// should match the slider axis.
        /// </summary>
        public AxisFlags MovableAxes
        {
            get => movableAxes;
            set => movableAxes = value;
        }

        [SerializeField]
        [Tooltip("For the movable axes, what distance should be moved on each OnMove event?")]
        private float onMoveDelta = 0.01f;

        /// <summary>
        /// For the movable axes, what distance should be moved on each OnMove event?
        /// </summary>
        /// <remarks>
        /// This is applied along object's local axes, but scaled in world space distance.
        /// </remarks>
        public float OnMoveDelta => onMoveDelta;

        private IProxyInteractor proxyInteractor;

        private List<IXRInteractor> interactorQueryList = new List<IXRInteractor>();

        /// <summary>
        /// The associated <see cref="XRBaseInteractable"/> on behalf of which
        /// this shim will translate input events.
        /// </summary>
        protected IProxyInteractor ProxyInteractor => proxyInteractor;

        private XRInteractionManager interactionManager;

        /// <summary>
        /// The interaction manager to use to query interactors and their registration events.
        /// Currently protected internal, may be exposed in a future update.
        /// </summary>
        internal protected XRInteractionManager InteractionManager
        {
            get
            {
                if (interactionManager == null)
                {
                    // First, check if we can get the reference from our interactable.
                    if (ThisInteractable is XRBaseInteractable baseInteractable)
                    {
                        interactionManager = baseInteractable.interactionManager;
                    }
                    else
                    {
                        // Otherwise, go find one.
                        interactionManager = ComponentCache<XRInteractionManager>.FindFirstActiveInstance();
                    }
                }

                return interactionManager;
            }
            set => interactionManager = value;
        }

#if UNITY_EDITOR
        /// <summary>
        /// A Unity Editor only event function that is called when the script is loaded or a value changes in the Unity Inspector.
        /// </summary>
        protected override void OnValidate()
        {   
            base.OnValidate();
            
            // Validate that no transition type is set. You shouldn't be using this
            // for any sort of UI visuals; use a StatefulInteractable and a 
            // StateVisualizer instead, even for UI.
            transition = UnityEngine.UI.Selectable.Transition.None;
        }
#endif

        /// <summary>
        /// A Unity event function that is called when an enabled script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (ThisInteractable != null && InteractionManager != null)
            {
                InteractionManager.interactorRegistered += OnInteractorRegistered;
                InteractionManager.interactorUnregistered += OnInteractorUnregistered;
            }

            // Some interactors (all?) get registered before we have a chance to subscribe to the
            // registration events. As a result; let's manually scrape the manager for the proxy interactor.
            if (proxyInteractor == null && ThisInteractable != null && InteractionManager != null)
            {
                // Go find the proxy interactor that this shim is associated with.
                InteractionManager.GetRegisteredInteractors(interactorQueryList);

                foreach (var interactor in interactorQueryList)
                {
                    if (interactor is IProxyInteractor proxy)
                    {
                        proxyInteractor = proxy;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// A Unity event function that is called when the script component has been disabled.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            // If the object is disabled, make sure not to keep hovering it!
            // We won't get pointerExit calls when the object is disabled.
            if (ProxyInteractor != null)
            {
                ProxyInteractor.EndHover(ThisInteractable as IXRHoverInteractable);
            }
        }

        /// <summary>
        /// Called when a an <see cref="IXRInteractor"/> is registered with a Unity <see cref="XRInteractionManager"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="InteractorRegisteredEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <param name="args">The <see cref="InteractorRegisteredEventArgs"/> holding the event data associated with the event when an <see cref="IXRInteractor"/> is registered with an <see cref="XRInteractionManager"/>.</param>
        protected virtual void OnInteractorRegistered(InteractorRegisteredEventArgs args)
        {
            if (args.interactorObject is IProxyInteractor)
            {
                proxyInteractor = args.interactorObject as IProxyInteractor;
            }
        }

        /// <summary>
        /// Called when a an <see cref="IXRInteractor"/> is unregistered with a Unity <see cref="XRInteractionManager"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="InteractorRegisteredEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <param name="args">The <see cref="InteractorRegisteredEventArgs"/> holding the event data associated with the event when an <see cref="IXRInteractor"/> is unregistered with an <see cref="XRInteractionManager"/>.</param>
        protected virtual void OnInteractorUnregistered(InteractorUnregisteredEventArgs args)
        {
            if (args.interactorObject is IProxyInteractor unregisteredProxyInteractor &&
                proxyInteractor == unregisteredProxyInteractor)
            {
                proxyInteractor = null;
            }
        }

        /// <summary>
        /// Test if the given event data corresponds to a Unity XRI interactor.
        /// </summary>
        /// <remarks>
        /// Used to filter XRI interactions out of all canvas input, so that duplicate events
        /// are not triggered on hybrid/unified UX.
        /// </remarks>
        /// <returns>
        /// <see langword="true"/> if the provided pointer event data corresponds to a Unity XRI interactor,
        /// <see langword="false"/> if it corresponds to mouse, keyboard, gamepad, or touch.
        /// </returns>
        protected bool IsXRUIEvent(PointerEventData pointerEventData)
        {
            return pointerEventData.currentInputModule is XRUIInputModule xrui && xrui.GetInteractor(pointerEventData.pointerId) != null;
        }

        #region Canvas/UnityUI Event Hooks

        /// <summary>
        /// Evaluate current state and transition to appropriate state.
        /// New state could be pressed or hover depending on pressed state.
        /// </summary>
        /// <param name="pointerEventData">The event data associated with the event.</param>
        public override void OnPointerEnter(PointerEventData pointerEventData)
        {
            base.OnPointerEnter(pointerEventData);

            // Reject all incoming UnityUI input if it originates from
            // an XRI interactor. The Interactable itself will handle those inputs,
            // and we don't want to duplicate them.
            if (IsXRUIEvent(pointerEventData)) { return; }

            if (ThisInteractable is IXRHoverInteractable hoverInteractable &&
                ProxyInteractor != null)
            {
                ProxyInteractor.StartHover(hoverInteractable, pointerEventData.pointerCurrentRaycast.worldPosition);
            }
        }

        /// <summary>
        /// Evaluate current state and transition to normal state.
        /// </summary>
        /// <param name="pointerEventData">The event data associated with the event.</param>
        public override void OnPointerExit(PointerEventData pointerEventData)
        {
            base.OnPointerExit(pointerEventData);

            // Reject all incoming UnityUI input if it originates from
            // an XRI interactor. The Interactable itself will handle those inputs,
            // and we don't want to duplicate them.
            if (IsXRUIEvent(pointerEventData)) { return; }

            // Just use the Deselect handler to clear our hover.
            // This ensures that the button will not stay "selected" (read: hovered)
            // after we click with a mouse.
            OnDeselect(pointerEventData);
        }

        /// <summary>
        /// Evaluate current state and transition to pressed state.
        /// </summary>
        /// <param name="pointerEventData">The event data associated with the event.</param>
        public override void OnPointerDown(PointerEventData pointerEventData)
        {
            base.OnPointerDown(pointerEventData);

            // Only left-clicks are selects.
            if (pointerEventData.button != PointerEventData.InputButton.Left) { return; }

            // Reject all incoming UnityUI input if it originates from
            // an XRI interactor. The Interactable itself will handle those inputs,
            // and we don't want to duplicate them.
            if (IsXRUIEvent(pointerEventData)) { return; }

            if (ThisInteractable is IXRSelectInteractable selectInteractable &&
                ProxyInteractor != null)
            {
                ProxyInteractor.StartSelect(selectInteractable, pointerEventData.pointerCurrentRaycast.worldPosition);
            }
        }

        /// <summary>
        /// Evaluate eventData and transition to appropriate state.
        /// </summary>
        /// <param name="pointerEventData">The event data associated with the event.</param>
        public override void OnPointerUp(PointerEventData pointerEventData)
        {
            base.OnPointerUp(pointerEventData);

            // Only left-clicks are selects.
            if (pointerEventData.button != PointerEventData.InputButton.Left) { return; }

            // Reject all incoming UnityUI input if it originates from
            // an XRI interactor. The Interactable itself will handle those inputs,
            // and we don't want to duplicate them.
            if (IsXRUIEvent(pointerEventData)) { return; }

            if (ThisInteractable is IXRSelectInteractable selectInteractable &&
                ProxyInteractor != null)
            {
                // Cancel the click if the event is a drag event, or if we've stopped hovering the interactable
                // (i.e., we've rolled off)
                bool shouldCancel = pointerEventData.dragging || !ProxyInteractor.IsHovering(ThisInteractable as IXRHoverInteractable);

                ProxyInteractor.EndSelect(selectInteractable, shouldCancel);
            }
        }

        /// <summary>
        /// Set selection and transition to appropriate state.
        /// </summary>
        /// <param name="eventData">The event data associated with the event.</param>
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            if (ThisInteractable is IXRHoverInteractable hoverInteractable &&
                ProxyInteractor != null)
            {
                ProxyInteractor.StartHover(hoverInteractable);
            }
        }

        /// <summary>
        /// Unset selection and transition to appropriate state.
        /// </summary>
        /// <param name="eventData">The event data associated with the event.</param>
        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);

            if (ThisInteractable is IXRHoverInteractable hoverInteractable &&
                ProxyInteractor != null)
            {
                ProxyInteractor.EndHover(hoverInteractable);
            }
        }

        /// <summary>
        /// Determine in which of the 4 move directions the next selectable object should be found.
        /// </summary>
        /// <param name="eventData">The event data associated with the event.</param>
        public override void OnMove(AxisEventData eventData)
        {
            // Use base OnMove if object is either inactive or un-interactable
            if (!IsActive() || !IsInteractable())
            {
                base.OnMove(eventData);
                return;
            }

            // If the current OnMove's axis has been set as a movable axis
            // (i.e., used for manipulation instead of navigation), we'll )
            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    if (((MovableAxes & AxisFlags.XAxis) == AxisFlags.XAxis))
                        StartCoroutine(Move(Vector3.right * -1.0f));
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Right:
                    if (((MovableAxes & AxisFlags.XAxis) == AxisFlags.XAxis))
                        StartCoroutine(Move(Vector3.right));
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Up:
                    if (((MovableAxes & AxisFlags.YAxis) == AxisFlags.YAxis))
                        StartCoroutine(Move(Vector3.up));
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Down:
                    if (((MovableAxes & AxisFlags.YAxis) == AxisFlags.YAxis))
                        StartCoroutine(Move(Vector3.up * -1.0f));
                    else
                        base.OnMove(eventData);
                    break;
            }
        }

        /// <summary>
        /// OnMoves on UnityUI elements are equivalent to selecting, moving, and deselecting
        /// with XRI. This coroutine has a single-frame delay between these actions to ensure
        /// XRI synchronizes and recognizes these inputs. The object-local delta is scaled by
        /// the <see cref="OnMoveDelta"/> value.
        /// </summary>
        /// <remarks>
        /// We may/will be implementing true directional input on our interactables instead of this.
        /// This implements things like slider input by literally moving the attachTransform of the
        /// proxy interactor. In the future, things like Sliders may implement some kind of interface
        /// that will enable directional/discrete input.
        /// </remarks>
        protected IEnumerator Move(Vector3 objectLocalDelta)
        {
            if (ThisInteractable is IXRSelectInteractable selectInteractable &&
                ProxyInteractor != null)
            {
                bool interactorWasSelecting = ProxyInteractor.IsSelecting(selectInteractable);

                if (!interactorWasSelecting)
                {
                    ProxyInteractor.StartSelect(selectInteractable);
                    yield return null;
                }

                Vector3 globalDelta = transform.TransformDirection(objectLocalDelta) * OnMoveDelta;

                ProxyInteractor.UpdateSelect(selectInteractable, ProxyInteractor.transform.position + globalDelta);
                yield return null;

                if (!interactorWasSelecting)
                {
                    // If we were already selecting this interactable, then skip the start/end select.
                    ProxyInteractor.EndSelect(selectInteractable);
                }
            }
        }
        
        /// <summary>
        /// Called when the Unity UGUI element is selected.
        /// </summary>
        public virtual void OnSubmit(BaseEventData eventData)
        {
            // Map submit event to XRI Select, but using a coroutine
            // to simulate a selection/deselection. This is the
            // same way that UnityUI Buttons do this.
            // Gamepads fire submit event on pressing a button.

            // This will start the coroutine, as well as manage
            // the internal Selectable state transitions and notify
            // the XRI proxy interactor.
            Click();
        }

        #endregion Canvas/UnityUI Event Hooks

        #region Accessibility

        /// <summary>
        /// Click the interactable. This will result in the interactable being selected and
        /// then deselected after a short period of time. This is the same behavior that occurs
        /// when this adapter receives a Unity UGUI <see cref="OnSubmit"/> event.
        /// </summary>
        /// <remarks> 
        /// This is useful for accessibility features that need to induce a full
        /// submit like behavior from a Unity event that can't invoke the submit event directly.
        /// </remarks>
        public void Click()
        {
            // Make the Canvas state machine happy, in the
            // off chance anyone is listening to canvas states.
            DoStateTransition(SelectionState.Pressed, false);

            if (ThisInteractable is IXRSelectInteractable selectInteractable &&
                ProxyInteractor != null)
            {
                ProxyInteractor.StartSelect(selectInteractable);
            }

            // We run this coroutine to deselect the object
            // after a brief moment has passed.
            StartCoroutine(OnFinishSubmit());
        }

        #endregion

        private IEnumerator OnFinishSubmit()
        {
            // TODO: configurable.
            yield return new WaitForSeconds(0.1f);

            // Again, making the Canvas state machine happy.
            DoStateTransition(currentSelectionState, false);

            if (ThisInteractable is IXRSelectInteractable selectInteractable &&
                ProxyInteractor != null)
            {
                ProxyInteractor.EndSelect(selectInteractable);
            }
        }

    }
}
