using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiigaikVkBot.Utils
{
    /// <summary>
    /// Генерация реакций на невалидные команды для бота
    /// </summary>
    public static class InvMsgReactProv
    {
        private static string[] obsceneLexicsDict =
        {
            "хуй",
            "хуе",
            "бля",
            "еба",
            "пизд",
            "пздц",
            "хер",
            "идиот",
            "дибил",
            "дебил",
            "урод",
            "мразь",
            "сук"
        };

        private static string[] normalResponses =
        {
            "Прошу прощения, я вас не понял. Даже не знаю, что и ответить :-(",
            "Моя твоя не понимать.",
            "Да",
            "Я не могу помочь вам с этим вопросом.",
            "Хороший вопрос! :-)",
            "Спасибо, что уделили нам время! Вы очень важны для нас!",
        };

        private static string[] obsRespones =
        {
            "Обратитесь к онкологу. У вас рак чувства юмора",
            "Я не знаю, чем ты руководствовался, но логику я исключаю сразу.",
            "А ты умнее. чем о тебе говорят!"
        };

        private static Random rnd = new Random();

        /// <summary>
        /// Автоматически определить характер сообщения и выдать на него ответ
        /// </summary>
        /// <returns>Возвращает сообщение, которое соответствует тону спрашивающего</returns>
        public static string ProcMsg(string msg)
        {
            foreach (string innorm_word in obsceneLexicsDict)
                if (msg.Contains(innorm_word))
                    return invMsgObs();

            return invMsgNormal();
        }

        /// <summary>
        /// Возвращает нормальный ответ, который показывает недопонимание со стороны бота
        /// </summary>
        /// <returns>Ответ в виде строки типа string</returns>
        private static string invMsgNormal() => normalResponses[rnd.Next(0, normalResponses.Length - 1)];

        /// <summary>
        /// Возвращает хамский ответ
        /// </summary>
        /// <returns>Ответ в виде строки типа string</returns>
        private static string invMsgObs() => obsRespones[rnd.Next(0, obsRespones.Length - 1)];
    }
}
