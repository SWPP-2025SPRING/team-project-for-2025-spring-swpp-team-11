using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Serialization;

public enum StageState
{
   Ready,
   Started,
   Finished,
}
public class StageManager : MonoBehaviour
{
   public int stageNumber;
   
   [SerializeField] private InGameUI inGameUI;
   [FormerlySerializedAs("playableDirector")] [SerializeField] private PlayableDirector openingCutScene;
   [SerializeField] private PlayableDirector clearCutsceneDirector;

   public StageState currentStageState = StageState.Ready;

   public GameObject resultUICanvas;
   public GameObject resultUI;
   public GameObject ingameUICanvas;
   public GameObject ingameUI;

   private void Start()
   {
      GameManager.Instance.InputManager.canControlPlayer = false;
      GameManager.Instance.InputManager.onEnterEvent.AddListener(OnSkip);
      openingCutScene.stopped += OnStartCutsceneFinished;
   }

   private void OnStartCutsceneFinished(PlayableDirector pd)
   {
      GameManager.Instance.InputManager.canControlPlayer = true;
      currentStageState = StageState.Started;
   }
   
   private void OnClearCutsceneFinished(PlayableDirector pd)
   {
      currentStageState = StageState.Started;
   }

   private void OnSkip(InputAction.CallbackContext ctx)
   {
      if (currentStageState == StageState.Ready)
      {
         openingCutScene.time = openingCutScene.duration;
         openingCutScene.Evaluate();
         GameManager.Instance.InputManager.onEnterEvent.RemoveListener(OnSkip);
      }
      
      if (currentStageState == StageState.Finished)
      {
         clearCutsceneDirector.time = clearCutsceneDirector.duration;
         clearCutsceneDirector.Evaluate();
         GameManager.Instance.InputManager.onEnterEvent.RemoveListener(OnSkip);
      }
   }

   public void Finish()
   {
      StartCoroutine(FinishCoroutine());
   }

   private IEnumerator FinishCoroutine()
   {
      GameManager.Instance.InputManager.canControlPlayer = false;
      
      ingameUI.GetComponent<InGameUI>().RemoveOnEscapeCallBack();
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
      
      ingameUICanvas.SetActive(false);
      ingameUICanvas.SetActive(false);
      float timeRecord = ingameUI.GetComponent<InGameUI>().GetElapsedTime();
      resultUI.gameObject.SetActive(true);

      var result = resultUI.GetComponent<ResultUI>();
      result.record = timeRecord;
      resultUICanvas.gameObject.SetActive(true);
      
      yield return StartCoroutine((GameManager.Instance.SceneLoadManager.FadeOut()));
      
      clearCutsceneDirector.Play();
      StartCoroutine((GameManager.Instance.SceneLoadManager.FadeIn()));

      yield return new WaitForSeconds(2f);
      StartCoroutine(result.Animate());
   }
}
