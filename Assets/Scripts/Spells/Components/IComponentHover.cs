
public interface IComponentHover
{
    void OnHoverEnter(SpellGridCell cell);
    void OnHoverExit(SpellGridCell cell);
}
public interface IComponentDisplayedText
{
    void SetDisplayedText(SpellGridCell cell);
}
