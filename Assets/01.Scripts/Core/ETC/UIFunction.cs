using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIFunction
{
    #region 이미지 이미지 겹치는지 확인
    public static bool IsImagesOverlapping(RectTransform rectTransform1, RectTransform rectTransform2)
    {
        Vector3[] corners1 = new Vector3[4];
        Vector3[] corners2 = new Vector3[4];

        rectTransform1.GetWorldCorners(corners1);
        rectTransform2.GetWorldCorners(corners2);

        for (int i = 0; i < 4; i++)
        {
            if (IsPointInsideRect(corners1[i], rectTransform2) || IsPointInsideRect(corners2[i], rectTransform1))
            {
                return true;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            Vector3 start1 = corners1[i];
            Vector3 end1 = corners1[(i + 1) % 4];

            for (int j = 0; j < 4; j++)
            {
                Vector3 start2 = corners2[j];
                Vector3 end2 = corners2[(j + 1) % 4];

                if (AreLineSegmentsIntersecting(start1, end1, start2, end2))
                {
                    return true;
                }
            }
        }

        return false;
    }
    private static bool IsPointInsideRect(Vector3 point, RectTransform rectTransform)
    {
        Rect rect = new Rect(rectTransform.position.x - rectTransform.rect.width / 2,
                             rectTransform.position.y - rectTransform.rect.height / 2,
                             rectTransform.rect.width,
                             rectTransform.rect.height);

        return rect.Contains(point);
    }
    private static bool AreLineSegmentsIntersecting(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2)
    {
        return ArePointsOnDifferentSides(start1, end1, start2, end2) &&
               ArePointsOnDifferentSides(start2, end2, start1, end1);
    }
    private static bool ArePointsOnDifferentSides(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b)
    {
        float sign1 = Mathf.Sign(Vector3.Cross(b - a, p1 - a).z);
        float sign2 = Mathf.Sign(Vector3.Cross(b - a, p2 - a).z);

        return sign1 != sign2;
    }
    #endregion

    #region 마우스가 이미지 위에 있는지 확인
    public static bool IsMouseInRectTransform(Vector2 mousePosition, RectTransform rectTrm)
    {
        Vector3[] corners = new Vector3[4];
        rectTrm.GetWorldCorners(corners);

        return mousePosition.x > corners[0].x &&
               mousePosition.x < corners[2].x &&
               mousePosition.y > corners[0].y &&
               mousePosition.y < corners[2].y;
    }
    #endregion
}

