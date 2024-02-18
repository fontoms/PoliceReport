namespace LogicLayer.Grade
{
    public interface IGradeDao
    {
        void Add(Grade grade);
        void Remove(Grade grade);
        List<Grade> GetAll();
    }
}
