// JsonLoader.cs
using UnityEngine;
using System;
using Game.Core; // ManagerBase가 Game.Core 네임스페이스에 있다고 가정

/// <summary>
/// Resources 폴더에서 JSON 파일을 로드하고 지정된 타입으로 역직렬화하는 범용 클래스입니다.
/// 특정 데이터 타입에 의존하지 않으며, 데이터 저장 로직을 포함하지 않습니다.
/// 이 컴포넌트는 Unity 씬에 존재해야 합니다.
/// </summary>
public class JsonLoader //: MonoBehaviour // ManagerBase 상속 유지
{


    /// <summary>
    /// Resources 폴더에서 지정된 JSON 파일을 로드하고 파싱하여 T 타입 객체로 반환합니다.
    /// </summary>
    /// <typeparam name="T">JSON 데이터를 역직렬화할 대상 타입입니다.
    /// 이 타입은 반드시 [Serializable] 어트리뷰트가 필요하며, JSON 파일의 구조와 일치해야 합니다.</typeparam>
    /// <param name="path">JSON 파일의 Resources 경로 (확장자 제외). 예: "Data/PawnTypes".</param>
    /// <returns>역직렬화된 T 타입 객체입니다. 실패 시 null을 반환합니다.</returns>
    public T LoadJson<T>(string path) where T : class
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(path);
        if (jsonTextAsset == null)
        {
            Debug.LogError($"JsonLoader: JSON 파일 '{path}.json'을 Resources에서 찾을 수 없습니다.");
            return null;
        }

        try
        {
            T result = JsonUtility.FromJson<T>(jsonTextAsset.text);
            Debug.Log($"JsonLoader: '{path}'에서 '{typeof(T).Name}' 데이터 로드 완료.");
            return result;
        }
        catch (Exception e)
        {
            Debug.LogError($"JsonLoader: JSON 파싱 오류 발생 for '{path}' ({typeof(T).Name}): {e.Message}\n{e.StackTrace}");
            return null;
        }
    }
}