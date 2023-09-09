using System.Text;
using HtmlAgilityPack;
using MiigaikVkBot.Timetable.Models;

namespace MiigaikVkBot.Timetable.Implementation
{
    public class LegacyShedule : BaseShedule
    {
        /// <summary>
        /// Ревёрс недели.
        /// </summary>
        private bool _reverseWeek = false;

        /// <summary>
        /// Ссылка на страницу, откуда будет грузиться расписание
        /// </summary>
        public string URL = "";

        /// <summary>
        /// Определяет, какая сейчас неделя - нижняя или верхняя
        /// </summary>
        public bool isLowerWeek;

        /// <summary>
        /// Инициализирует экземпляр Shedule и все поля внутри
        /// </summary>
        public LegacyShedule(bool reverseWeek)
        {
            _reverseWeek = reverseWeek;
            Clear();
        }

        public override void Clear()
        {
            Monday = new Day();
            Monday.Timetable.Add(new Subject()
            {
                SubjectName = "Расписание отсутствует"
            });

            Tuesday = new Day();
            Tuesday.Timetable.Add(new Subject()
            {
                SubjectName = "Расписание отсутствует"
            });

            Wednesday = new Day();
            Wednesday.Timetable.Add(new Subject()
            {
                SubjectName = "Расписание отсутствует"
            });

            Thursday = new Day();
            Thursday.Timetable.Add(new Subject()
            {
                SubjectName = "Расписание отсутствует"
            });

            Friday = new Day();
            Friday.Timetable.Add(new Subject()
            {
                SubjectName = "Расписание отсутствует"
            });

            Saturday = new Day();
            Saturday.Timetable.Add(new Subject()
            {
                SubjectName = "Расписание отсутствует"
            });

            Sunday = new Day();
            Sunday.Timetable.Add(new Subject()
            {
                SubjectName = "Расписание отсутствует"
            });
        }

        /// <summary>
        /// Вычисляет тип недели - верхняя или нижняя
        /// </summary>
        public override bool IsLower(DateTime inp)
        {
            bool res = ((int)(inp - new DateTime(2019, 9, 1)).TotalDays + 13) / 7 % 2 == 0;
            return !_reverseWeek ? res : !res;
        }

        public override void UpdateTimetable()
        {
            // Решаем проблему с кодировкой. Веб-страница использует кодировку Windows-1251
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Загружаем сайт
            HtmlWeb webDoc = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = System.Text.Encoding.GetEncoding("UTF-8")
            };

            HtmlDocument doc = webDoc.Load(URL);

            // Отсеиваем ненужные ноды
            HtmlNodeCollection destinationNodes = doc.DocumentNode.SelectNodes("//table[@class='t']//tr");
            List<HtmlNodeCollection> dayNodes = new List<HtmlNodeCollection>();
            foreach (HtmlNode node in destinationNodes)
            {
                dayNodes.Add(node.SelectNodes("node()"));
            }

            // Если пустая страница
            if (dayNodes.Count == 0)
            {
                // Отключаем дальнейшую загрузку
                return;
            }
            
            // Удалим шапку таблицы
            dayNodes.RemoveAt(0);

            // Предварительно очистим старые записи о предметах
            Monday.Clear();
            Tuesday.Clear();
            Wednesday.Clear();
            Thursday.Clear();
            Friday.Clear();
            Saturday.Clear();

            // День, который сейчас будет заполняться
            Day fillingDay = null;

            for (int i = 0; i < dayNodes.Count; i++)
            {

                // Если есть только один элемент, то это заголовок с названием дня недели
                if (dayNodes[i].Count == 1)
                {
                    switch (dayNodes[i][0].InnerText.ToLower())
                    {
                        case "понедельник&nbsp;":
                            fillingDay = this.Monday;
                            break;

                        case "вторник&nbsp;":
                            fillingDay = this.Tuesday;
                            break;

                        case "среда&nbsp;":
                            fillingDay = this.Wednesday;
                            break;

                        case "четверг&nbsp;":
                            fillingDay = this.Thursday;
                            break;

                        case "пятница&nbsp;":
                            fillingDay = this.Friday;
                            break;

                        case "суббота&nbsp;":
                            fillingDay = this.Saturday;
                            break;

                        case "воскресенье&nbsp;":
                            fillingDay = this.Sunday;
                            break;
                    }

                    continue;
                }
                // Иначе это информация о паре
                else
                {
                    // Пропускаем бракованные элементы
                    if (dayNodes[i].Count != 9)
                        continue;

                    // 0 - день недели
                    // 1 - номер пары
                    // 2 - верхняя/нижняя неделя
                    // 3 - номер подгруппы
                    // 4 - наименование пары
                    // 5 - имя препода-преподов
                    // 6 - аудитория
                    // 7 - тип (практика/лекция)
                    // 8 - &nbsp; или комментарии

                    int subjNumber = Convert.ToInt32(dayNodes[i][1].InnerText[0].ToString());

                    bool isLowerWeek = (dayNodes[i][2].InnerText[0] == 'н' || dayNodes[i][2].InnerText[0] == 'Н') ? true : false; // Такое решение из-за того, что могут мешаться дополнительные управляющие символы, а-ля \n\r, а также большие/маленькие буквы

                    StringBuilder subjNameSB = new StringBuilder();
                    subjNameSB.Append(dayNodes[i][4].InnerText.Trim());
                    subjNameSB[0] = char.ToUpper(subjNameSB[0]);

                    int subjGroup = 0;
                    if (dayNodes[i][3].InnerText.Trim() != "&nbsp;")
                        subjGroup = Convert.ToInt32(dayNodes[i][3].InnerText);

                    string educatorsNames = dayNodes[i][5].InnerText.TrimStart();
                    string auditory = dayNodes[i][6].InnerText;

                    StringBuilder subjectTypeSB = new StringBuilder();
                    subjectTypeSB.Append(dayNodes[i][7].InnerText);
                    subjectTypeSB[0] = char.ToUpper(subjectTypeSB[0]);

                    string comment = string.Empty;
                    if (dayNodes[i][8].InnerText != "&nbsp;")
                        comment = dayNodes[i][8].InnerText.Replace("&nbsp;", "");

                    // Соберём объект с информацией о паре и поместим его в расписание соответствующего дня
                    Subject subject = new Subject()
                    {
                        SubjectNumber = subjNumber,
                        WeekType = isLowerWeek ? WeekType.Lower : WeekType.Upper,
                        SubjectName = subjNameSB.ToString(),
                        GroupNumber = subjGroup,
                        EducatorName = educatorsNames,
                        Auditory = auditory,
                        SubjectType = subjectTypeSB.ToString(),
                        Comment = comment
                    };

                    fillingDay.Timetable.Add(subject);
                }
            }
        }
    }
}
