using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.DbStuff.Model;
using WebMaze.DbStuff.Model.Police;
using WebMaze.DbStuff.Model.Police.Enums;

namespace WebMaze.Infrastructure
{
    public static class PoliceNotificationsFactory
    {
        public static PoliceNotification[] AddViolation(CitizenUser blamedUser, CitizenUser blamingUser)
        {
            var blaming = GetDefault(blamingUser);
            blaming.Title = "Ваше заявление принято";
            blaming.Message = $"Ваше заявление насчет пользователя {blamedUser.FirstName} {blamedUser.LastName} принято";

            var blamed = GetDefault(blamedUser);
            blamed.Title = "Внимание, к вам поступила жалоба";
            blamed.Message = $"Пользователь {blamingUser.FirstName} {blamingUser.LastName} отправил к вам жалобу. " +
                $"Чтобы проверить статус жалобы, перейдите по ссылке";

            return new PoliceNotification[] { blaming, blamed };
        }

        public static PoliceNotification OfficerTookViolation(CitizenUser user)
        {
            var result = GetDefault(user);
            result.Title = "Ваше заявление было обработано";
            result.Message = "Ваше заявление было принято. Судья в скором времени решит ваш вопрос";
            return result;
        }

        public static PoliceNotification OfficerDeniedFromViolation(CitizenUser user)
        {
            var result = GetDefault(user);
            result.Title = "Суд отказался от дела.";
            result.Message = "Судья отказался от данного дела. В данный момент заявлению " +
                "присвоен статус ожидания. В скором времени за дело возьмется другой судья";
            return result;
        }

        public static PoliceNotification[] DenyViolation(CitizenUser blamedUser, CitizenUser blamingUser)
        {
            var blaming = GetDefault(blamingUser);
            blaming.Title = "Суд принял решение отозвать заявление";
            blaming.Message = "Суд отказался от рассмотрения вашего дела. Доп. информация на странице дела";

            var blamed = GetDefault(blamedUser);
            blamed.Title = "Дело закрыто в вашу пользу";
            blamed.Message = "Поздравляем, суд решил, что вы невиновны и закрывает это дело. Доп. информация на странице дела";

            return new PoliceNotification[] { blaming, blamed };
        }

        public static PoliceNotification[] AcceptedViolation(CitizenUser blamedUser, CitizenUser blamingUser)
        {
            var blaming = GetDefault(blamingUser);
            blaming.Title = "Дело закрыто в вашу пользу";
            blaming.Message = "Суд принял ваши жалобы. Доп. информация на странице дела";

            var blamed = GetDefault(blamedUser);
            blamed.Title = "Дело закрыто не в вашу пользу";
            blamed.Message = "К сожалению, суд принял решение принять жалобы в вашу сторону. Доп. информация на странице дела";

            return new PoliceNotification[] { blaming, blamed };
        }

        private static PoliceNotification GetDefault(CitizenUser toUser)
        {
            return new PoliceNotification()
            {
                CurrentStatus = ReadStatus.WasNotRead,
                Date = DateTime.Now,
                ToUser = toUser
            };
        }
    }
}
