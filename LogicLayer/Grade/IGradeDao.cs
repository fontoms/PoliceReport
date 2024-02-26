namespace LogicLayer.Grade
{
    public interface IGradeDao
    {
        void Add(Grade grade);
        void Remove(Grade grade);
        void Update(Grade grade);
        List<Grade> GetAll();
    }
}
