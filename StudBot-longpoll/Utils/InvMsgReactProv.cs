using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudBot.Utils
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
            "Прошу прощения. Мой хозяин слишком ленив, чтобы запрограммировать команду, которую вы от меня требуете.",
            "Я не могу помочь вам с этим вопросом.",
            "Хороший вопрос! :-)",
            "Спасибо, что уделили нам время! Вы очень важны для нас!",
            "Прости, меня в гугле забанили, не могу узнать ответ на твой вопрос",
            "Ай... СЛОООЖЖЖЖНА!!!"
        };

        private static string[] obsRespones =
        {
            "Видел я дерьмо, но в первый раз вижу, чтобы оно мне хамило.",
            "Обратитесь к онкологу. У вас рак чувства юмора",
            "Попробуй напрячь свои извилины - это не больно. Может быть выдашь что-нибудь более умное.",
            "Я не знаю, чем ты руководствовался, но логику я исключаю сразу.",
            "А ты умнее. чем о тебе говорят!",
            "Я так понимаю, что твой мозг идеально гладкий...",
            "А ты знаешь, что такое \"Апперкот\"? Просто ты сейчас в шаге от этого великого познания.",
            "Отлично сказано! Бухал вчера?",
            "Унижать Вас слишком гнусно с моей стороны, Вас жизнь и так не хило потрепала.",
            "Пойди, приляг. Желательно на рельсы."
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
