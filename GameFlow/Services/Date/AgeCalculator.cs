namespace GameFlow.Services.Date;

public class AgeCalculator : IAgeCalculatorService
{
    public int CalculateAge(DateTime birthDate)
    {
        int dif = DateTime.Now.Year - birthDate.Year;
        if (DateTime.Now.Month < birthDate.Month)
        {
            --dif;
        }
        else if (DateTime.Now.Month == birthDate.Month)
        {
            if (DateTime.Now.Day < birthDate.Day)
            {
                --dif;
            }
        }

        return dif;
    }
}