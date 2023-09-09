using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MiigaikVkBot.Converters;
using MiigaikVkBot.Timetable;
using MiigaikVkBot.Timetable.Implementation;
using MiigaikVkBot.Utils;
using VkNet;
using VkNet.Model;

namespace MiigaikVkBot.Responser
{
    public class Reponser : BaseResponser
    {
        /// <summary>
        /// Ссылка на страницу с расписанием
        /// </summary>
        private string GroupUrl { get; init; }

        /// <summary>
        /// Расписание группы
        /// </summary>
        private BaseShedule _shedule;

        /// <summary>
        /// Дата последнего обновления
        /// </summary>
        private DateTime _lastUpdateTime;

        /// <summary>
        /// Конструктор клавиатуры
        /// </summary>
        private readonly KeyboardBuilder _keyboardBuilder = new KeyboardBuilder(false);

        /// <summary>
        /// Словарь с ссылками на пары
        /// </summary>
        private readonly ConcurrentDictionary<string, string> _urLs = new();

        public Reponser(string group_url, ulong _groupId, VkApi _vkApi, bool reverseWeek) : base(_groupId, _vkApi)
        {
            GroupUrl = group_url;
            
            _shedule = new Shedule(GroupUrl);
            _shedule.UpdateTimetable();
            _lastUpdateTime = DateTimeProvider.Now;

            _keyboardBuilder.AddButton("Сегодня", "", VkNet.Enums.StringEnums.KeyboardButtonColor.Primary);
            _keyboardBuilder.AddButton("Завтра", "", VkNet.Enums.StringEnums.KeyboardButtonColor.Primary);
            _keyboardBuilder.AddLine();
            _keyboardBuilder.AddButton("Сейчас", "", VkNet.Enums.StringEnums.KeyboardButtonColor.Negative);
            _keyboardBuilder.AddLine();
            _keyboardBuilder.AddButton("Понедельник", "", VkNet.Enums.StringEnums.KeyboardButtonColor.Default);
            _keyboardBuilder.AddButton("Вторник", "", VkNet.Enums.StringEnums.KeyboardButtonColor.Default);
            _keyboardBuilder.AddButton("Среда", "", VkNet.Enums.StringEnums.KeyboardButtonColor.Default);
            _keyboardBuilder.AddLine();
            _keyboardBuilder.AddButton("Четверг", "", VkNet.Enums.StringEnums.KeyboardButtonColor.Default);
            _keyboardBuilder.AddButton("Пятница", "", VkNet.Enums.StringEnums.KeyboardButtonColor.Default);
            _keyboardBuilder.AddButton("Суббота", "", VkNet.Enums.StringEnums.KeyboardButtonColor.Default);

            if (File.Exists("urls.json"))
            {
                try
                {
                    _urLs = Newtonsoft.Json.JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(File.ReadAllText("urls.json")) ?? new();
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Не удалось считать файл с ссылками");
                }
            }
            else
                File.Create("urls.json");
        }

        public override MessagesSendParams ConstructResponse(Message message)
        {
            if (DateTimeProvider.Now - _lastUpdateTime > TimeSpan.FromHours(1))
            {
                _shedule.UpdateTimetable();
                _lastUpdateTime = DateTimeProvider.Now;
            }

            string response = "";
            string msg = message.Body.ToLower();

            if (msg.Contains("|||"))
            {
                return null;
            }

            if (msg.Contains("снилс"))
            {
                return new MessagesSendParams()
                {
                    Message = "OK",
                    RandomId = new DateTime().Millisecond,
                    UserId = message.UserId,
                    Keyboard = _keyboardBuilder.Build()
                };
            }


            if (msg.StartsWith("тема:"))
            {
                return new MessagesSendParams()
                {
                    Message = "OK",
                    RandomId = new DateTime().Millisecond,
                    UserId = message.UserId,
                    Keyboard = _keyboardBuilder.Build()
                };
            }    

            if (msg.StartsWith("ссылка"))
            {
                msg = message.Body;
                var strings = msg.Split('\n').Select(x => x).Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (strings.Count == 3)
                {
                    string subjName = strings[1].ToLower();
                    subjName = string.Join( " ", subjName.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries ));

                    _urLs.AddOrUpdate(subjName, strings[2], (key, value) => strings[2]);

                    File.WriteAllText("urls.json", Newtonsoft.Json.JsonConvert.SerializeObject(_urLs));
                    return new MessagesSendParams()
                    {
                        Message = "Успешно",
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = _keyboardBuilder.Build()
                    };
                }
                else
                {
                    return new MessagesSendParams()
                    {
                        Message = "Неверный формат",
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = _keyboardBuilder.Build()
                    };
                }
            }

            if (msg.StartsWith("удалить ссылку"))
            {
                var strings = msg.Split('\n').Select(x => x).Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (strings.Count == 2)
                {
                    string value = string.Empty;
                    
                    string subjName = strings[1].ToLower();
                    subjName = string.Join( " ", subjName.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries ));
                    
                    if (_urLs.TryGetValue(subjName, out value))
                        _urLs.Remove(subjName, out _);

                    return new MessagesSendParams()
                    {
                        Message = "Успешно",
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = _keyboardBuilder.Build()
                    };
                }
                else
                {
                    return new MessagesSendParams()
                    {
                        Message = "Неверный формат",
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = _keyboardBuilder.Build()
                    };
                }
            }

            switch (msg)
            {
                case "сегодня":
                    if (DateTimeProvider.Now.DayOfWeek != DayOfWeek.Sunday)
                        response = SheduleFormat.GetSheduleOn(DateTimeProvider.Now, _shedule, false, _urLs);
                    else
                        return new MessagesSendParams()
                        {
                            Message = "ВОСКРЕСЕНЬЕ:\n\nВыходной день. Пар не должно быть.",
                            RandomId = new DateTime().Millisecond,
                            UserId = message.UserId,
                            Keyboard = _keyboardBuilder.Build()
                        };
                    break;
                case "завтра":
                    if (DateTimeProvider.Now.DayOfWeek != DayOfWeek.Saturday)
                            response = SheduleFormat.GetSheduleOn(DateTimeProvider.Now.AddDays(1), _shedule, false, _urLs);
                    else
                        return new MessagesSendParams()
                        {
                            Message = "ВОСКРЕСЕНЬЕ:\n\nВыходной день. Пар не должно быть.",
                            RandomId = new DateTime().Millisecond,
                            UserId = message.UserId,
                            Keyboard = _keyboardBuilder.Build()
                        };
                    break;
                case "понедельник":
                case "вторник":
                case "среда":
                case "четверг":
                case "пятница":
                case "суббота":
                    DayOfWeek day = Converters.DayOfWeekConverter.FromStrToDOW(message.Body);
                    // на неделю вперёд
                    //response = Converters.SheduleFormat.GetSheduleOn((new DateTime(2019, 9, 2)).AddDays(day == DayOfWeek.Sunday ? 6 : (int)day - 1), IpdShedule, true);
                    response = Converters.SheduleFormat.GetSheduleOn((new DateTime(2019, 9, 2)).AddDays(day == DayOfWeek.Sunday ? 6 : (int)day - 1), _shedule, true, _urLs);
                    //response = Converters.SheduleFormat.GetSheduleOn(DateTimeProvider.Now, IpdShedule, true);
                    break;
                case "воскресенье":
                    return new MessagesSendParams()
                    {
                        Message = "ВОСКРЕСЕНЬЕ:\n\nВыходной день. Пар не должно быть.",
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = _keyboardBuilder.Build()
                    };
                case "начать":
                case "помощь":
                    return new MessagesSendParams()
                    {
                        Message = $"{Emoji.RedCircle()} SheduleBOT v{VersionInfo.Ver} {Emoji.RedCircle()}\n\nПоследние изменения:\n{VersionInfo.LastUpdates}\nОбо всех найденных ошибках сообщать: http://vk.com/flexlug \n Для того, чтобы увидеть расписание, введите соответствующий день недели.\nТакже можно вывести расписание на сегодняшний или завтрашний день. Вводите \"сегодня\" или \"завтра\" соответственно.",
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = _keyboardBuilder.Build()
                    };
                case "обновить расписание":
                    _shedule.UpdateTimetable();
                    return new MessagesSendParams()
                    {
                        Message = "Расписание успешно обновлено!",
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = _keyboardBuilder.Build()
                    };
                case "есть ли вебинары?":
                    return new MessagesSendParams()
                    {
                        Message = "Режим СДО выключен.",
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = _keyboardBuilder.Build()
                    };
                case "сейчас":
                    return new MessagesSendParams()
                    {
                        Message = CurrentSubjectGetter.GetSubject(_shedule, _urLs),
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = _keyboardBuilder.Build()
                    };
                default:
                    return new MessagesSendParams()
                    {
                        Message = InvMsgReactProv.ProcMsg(msg),
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = _keyboardBuilder.Build()
                    };
            }

            return new MessagesSendParams()
            {
                Message = response,
                RandomId = new DateTime().Millisecond,
                UserId = message.UserId,
                Keyboard = _keyboardBuilder.Build()
            };
        }
    }
}