using System;
using System.Text;
using System.Diagnostics;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;

class TreeLog{
	private readonly object lockObj = new object();
	
	private StringBuilder log;
	
	private int level;
	
	public string getLog(){
		lock(lockObj){
			return log.ToString();
		}
	}
	
	public void reset(){
		lock(lockObj){
			log = new StringBuilder();
			level = 0;
		}
	}
	
	public void deep(string s){
		write("<#> " + s);
		level++;
	}
	
	public void shallow(){
		level--;
		write("<!>");
	}
	
	public void write(string s){
		lock(lockObj){
			log.AppendLine(new string('	', level) + s);
		}
	}
	
	public void write(object s){
		write(s.ToString());
	}
}

class TimeTool{
    Stopwatch sw;
    double breakPoint;
	
    string[] categoryNames;
    double[] times;
	
    List<double[]> history = new List<double[]>();
	
    int timeWriting;
	
    int maxHistory = 1000;
	
	private readonly object lockObj = new object();
	
    public TimeTool(int m, params string[] c){
		maxHistory = m;
        sw = new Stopwatch();
        categoryNames = c;
    }
	
    public void tickStart(){
		lock(lockObj){
			sw.Restart();
			breakPoint = 0d;
			timeWriting = 0;
			times = new double[categoryNames.Length + 1];
		}
    }
	
    public void catEnd(){
		lock(lockObj){
			double t = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1e9;
			times[timeWriting] += t - breakPoint;
			breakPoint = t;
			timeWriting++;
		}
    }
	
	public void catEnd(int i){
		lock(lockObj){
			double t = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1e9;
			times[i] += t - breakPoint;
			breakPoint = t;
			timeWriting = i + 1;
		}
    }
	
    public void tickEnd(){
		lock(lockObj){
			sw.Stop();
			
			double totalTime = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1e9;
			times[categoryNames.Length] = totalTime;
			
			// Add the current times to the history
			history.Add((double[])times.Clone());
			if (history.Count > maxHistory) {
				history.RemoveAt(0); // Remove the oldest entry if over max size
			}
		}
    }
	
    public string lastTickInfo(){
		StringBuilder sb = new StringBuilder();
		
		lock(lockObj){
			int maxCategoryNameWidth = "Category".Length;
			for(int i = 0; i < categoryNames.Length; i++){
				if(categoryNames[i].Length > maxCategoryNameWidth){
					maxCategoryNameWidth = categoryNames[i].Length;
				}
			}
			maxCategoryNameWidth += 2;
			
			sb.AppendLine("Category".PadRight(maxCategoryNameWidth) + "│" + String.Format("{0,13}", "Time") + " │" + String.Format("{0,12}", "Percentage"));
			sb.AppendLine(new string('─', maxCategoryNameWidth) + "┤╶" + new string('─', 12) + "─┤╶" + new string('─', 11));
			
			for (int i = 0; i < categoryNames.Length; i++) {
				sb.AppendLine(categoryNames[i].PadRight(maxCategoryNameWidth) + "│" +  String.Format("{0,13:N0}", times[i]) + " │" + String.Format("{0,12:N2}%", 100d * times[i] / times[categoryNames.Length]));
			}
			sb.AppendLine(new string('─', maxCategoryNameWidth) + "┤╶" + new string('─', 12) + "─┤╶" + new string('─', 11));
			sb.AppendLine("Total".PadRight(maxCategoryNameWidth) + "│" + String.Format("{0,13:N0}", times[categoryNames.Length]) + " │");
		}
		
        return sb.ToString();
    }
	
    public string meanInfo(){
		StringBuilder sb = new StringBuilder();
		
		lock(lockObj){
			if (history.Count == 0) return "No data in history.";
			
			// Compute averages
			double[] sumTimes = new double[categoryNames.Length + 1];
			double[] averageTimes = new double[categoryNames.Length + 1];
			double[] maxTimes = new double[categoryNames.Length + 1];
			
			foreach(double[] record in history) {
				for(int i = 0; i < record.Length; i++){
					sumTimes[i] += record[i];
					
					if(maxTimes[i] < record[i]){
						maxTimes[i] = record[i];
					}
				}
			}
			
			for(int i = 0; i < categoryNames.Length + 1; i++){
				averageTimes[i] = sumTimes[i] / history.Count;
			}
			
			// Build output
			
			int maxCategoryNameWidth = "Category".Length;
			for(int i = 0; i < categoryNames.Length; i++){
				if(categoryNames[i].Length > maxCategoryNameWidth){
					maxCategoryNameWidth = categoryNames[i].Length;
				}
			}
			maxCategoryNameWidth += 2;
			
			sb.AppendLine("Category".PadRight(maxCategoryNameWidth) + "│" + String.Format("{0,13}", "Max") + " │" + String.Format("{0,13}", "Average") + " │" + String.Format("{0,12}", "Percentage"));
			sb.AppendLine(new string('─', maxCategoryNameWidth) + "┤╶" + new string('─', 12) + "─┤╶" + new string('─', 12) + "─┤╶" + new string('─', 12));
			
			for(int i = 0; i < categoryNames.Length; i++){
				sb.AppendLine(categoryNames[i].PadRight(maxCategoryNameWidth) + "│" + String.Format("{0,13:N0}", maxTimes[i]) + " │" + String.Format("{0,13:N0}", averageTimes[i]) + " │" + String.Format("{0,12:N2}%", 100d * sumTimes[i] /  sumTimes[categoryNames.Length]));
			}
			
			sb.AppendLine(new string('─', maxCategoryNameWidth) + "┤╶" + new string('─', 12) + "─┤╶" + new string('─', 12) + "─┤");
			sb.AppendLine("Total".PadRight(maxCategoryNameWidth) + "│" + String.Format("{0,13:N0}", maxTimes[categoryNames.Length]) + " │" + String.Format("{0,13:N0}", averageTimes[categoryNames.Length]) + " │");
			sb.AppendLine(new string('─', maxCategoryNameWidth) + "┤╶" + new string('─', 12) + "─┤╶" + new string('─', 12) + "─┤");
			sb.AppendLine("Ticks".PadRight(maxCategoryNameWidth) + "│" + String.Format("{0,13:N0}", history.Count) + " │" + String.Format("{0,10:N1} hz", 1000000000d / averageTimes[categoryNames.Length]) + " │");
		}
        
        return sb.ToString();
    }
}