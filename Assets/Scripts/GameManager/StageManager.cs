using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public enum StageState
{
   Ready,
   Started,
   Finished,
}
public class StageManager : MonoBehaviour
{
   [SerializeField] private InGameUI inGameUI;
   [SerializeField] private PlayableDirector playableDirector;

   public StageState currentStageState = StageState.Ready;

   private void Start()
   {
      GameManager.Instance.InputManager.canControlPlayer = false;
      GameManager.Instance.InputManager.onEnterEvent.AddListener(OnSkip);
      playableDirector.stopped += OnStartCutsceneFinished;
   }

   private void OnStartCutsceneFinished(PlayableDirector pd)
   {
      GameManager.Instance.InputManager.canControlPlayer = true;
      currentStageState = StageState.Started;
   }

   private void OnSkip(InputAction.CallbackContext ctx)
   {
      Debug.Log("ASD");
      playableDirector.time = playableDirector.duration;
      playableDirector.Evaluate();  
      GameManager.Instance.InputManager.onEnterEvent.RemoveListener(OnSkip);
   }
}
