using UnityEngine;
using System.Collections;

public interface ITool
{
    void Update(Vector3 cursorPosition);

    void ButtonADown();
    void ButtonAUp();
    void ButtonA();
    void ButtonBDown();
    void ButtonBUp();
    void ButtonB();
    void ButtonXDown();
    void ButtonXUp();
    void ButtonX();
}
