using MAUIEssentials.Models;
using Newtonsoft.Json;

namespace MAUIEssentials.AppCode.Helpers
{
    public static class ChartHelper
    {
        private static string GetCSS()
        {
            return @"
                <style>
                    body, html{
                        background: #fff;
                        overflow:hidden;
                        height:100%;
                        width:100%;
                    }
  
                    canvas{
                        background:#fff;
                        height:100%;
                        width:100%;
                    }

                </style>";
        }

        public static string GetStackBarChart(ChartConfig config)
        {
            if (config == null) {
                return string.Empty;
            }

            var json = JsonConvert.SerializeObject(config, new JsonSerializerSettings() {
                NullValueHandling = NullValueHandling.Ignore,

            });

            var style = GetCSS();
            var isRTL = Settings.AppLanguage?.Language == AppLanguage.Arabic;

            return $@"
                <script src=""https://cdnjs.cloudflare.com/ajax/libs/Chart.js/3.7.1/chart.min.js""></script>
                <meta name='viewport' content='width=device-width,initial-scale=1,maximum-scale=1'/>

                {style}

                <div>
                <canvas id=""stackChart""></canvas>
                </div>

                <script>

                var ctx = document.getElementById(""stackChart"").getContext('2d');
                var myChart = new Chart(ctx, {{
                  type: 'bar',
                  data: {json},
                options: {{
                    plugins: {{
                        legend: {{
                            position: 'bottom',
                            rtl: {isRTL.ToString().ToLower()}
                        }},
                    }},
                    layout: {{
                        padding: 10
                    }},
                    tooltips: {{
                      displayColors: true,
                      callbacks: {{
                        mode: 'x',
                      }},
                    }},
                    scales: {{
                      x: {{
                        stacked: true,
                        grid: {{
                          display: false,
                        }}
                      }},
                      y: {{
                        stacked: true,
                        ticks: {{
                          beginAtZero: true,
                          stepSize: 1,
                        }},
                        type: 'linear',
                      }}
                    }},
                    responsive: true,
                    maintainAspectRatio: false,
                  }}
                }});

                </script>
                ";
        }

        public static string GetPieChart(ChartConfig config)
        {
            if (config == null) {
                return string.Empty;
            }

            var json = JsonConvert.SerializeObject(config, new JsonSerializerSettings() {
                NullValueHandling = NullValueHandling.Ignore,

            });

            var style = GetCSS();
            var isRTL = Settings.AppLanguage?.Language == AppLanguage.Arabic;

            return $@"
                <script src=""https://cdnjs.cloudflare.com/ajax/libs/Chart.js/3.7.1/chart.min.js""></script>
                <meta name='viewport' content='width=device-width,initial-scale=1,maximum-scale=1'/>

                {style}

                <div>
                <canvas id=""pieChart""></canvas>
                </div>

                <script>

                var ctx = document.getElementById(""pieChart"").getContext('2d');
                var myChart = new Chart(ctx, {{
                  type: 'pie',
                  data: {json},
                options: {{
                    plugins: {{
                        legend: {{
                            position: 'bottom',
                            rtl: {isRTL.ToString().ToLower()}
                        }},
                    }},
                    layout: {{
                        padding: 10
                    }},
                    responsive: true,
                    maintainAspectRatio: true,
                  }}
                }});

                </script>
                ";
        }
	}

    public class ChartConfig
	{
        [JsonProperty("datasets")]
        public List<ChartDataSet>? Datasets { get; set; }

        [JsonProperty("labels")]
        public List<string>? Labels { get; set; }
	}

    public class ChartDataSet
	{
        [JsonProperty("label")]
        public string? Label { get; set; }

        [JsonProperty("backgroundColor")]
        public object? BackgroundColor { get; set; }

        [JsonProperty("borderColor")]
        public object? BorderColor { get; set; }

        [JsonProperty("data")]
        public List<double>? Data { get; set; }
    }
}
