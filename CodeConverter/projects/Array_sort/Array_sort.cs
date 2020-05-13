static void main()
{
	//creating new int array mas
	int[] mas  = new int[10];
	//creating new int variable i
	int i;
	//creating new int variable max
	int max;
	//Entering int variable max
	max = Convert.ToInt32(Console.ReadLine());
	//execute structure for
	for( i =0 ; i <10 ; i ++)
	{
		//Entering int variable mas[i]
		int mas[i] = Convert.ToInt32(Console.ReadLine());
	}
	//execute variable max
	max =mas[0];
	//execute structure for
	for( i =0 ; i <10 ; i ++)
	{
		//execute structure if
		if(mas[i]>max)
		{
		//execute variable max
		max =mas[i];
		}
	}
	//outputing variable max
	Console.WriteLine(max);
}
