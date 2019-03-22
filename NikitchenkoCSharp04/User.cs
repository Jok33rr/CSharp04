using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;


namespace NikitchenkoCSharp04 

{
    public class UserCreationException : Exception
    {
        public UserCreationException(string message)
            : base(message)
        {
        }
    }

    public class UnrealNameException : UserCreationException
    {
        public UnrealNameException(string message)
            : base(message)
        {
        }
    }

    public class UnrealSurnameException : UserCreationException
    {
        public UnrealSurnameException(string message)
            : base(message)
        {
        }
    }

    public class UnrealEmailException : UserCreationException
    {
        public UnrealEmailException(string givenEmail)
            : base($"Email {givenEmail} не є дійсним!")
        {
        }
    }

    public class UnrealBirthdayException : UserCreationException
    {
        public UnrealBirthdayException(DateTime birthday)
            : base($"Дата народження {birthday.ToShortDateString()} не є можливою!")
        {
        }
    }
    [Serializable]
    public class User
    {
        private const string DataFilepath = "userdata";
        private const string UserFileTemplate = "user{0}.bin";

        public string Name { get; private set; }

        public string Surname { get; private set; }

        public string Email { get; private set; }

        public DateTime Birthday { get; private set; }

        public User(string name, string surname, string email, DateTime birthday)
        {
            if (name.Length < 2)
            {
                throw new UnrealNameException($"Ім'я {name} занадто коротке!");
            }

            if (surname.Length < 3)
            {
                throw new UnrealSurnameException($"Прізвище {surname} занадто коротке!");
            }

            if (email.Length < 3 || email.Count(f => f == '@') != 1 ||
                (email.IndexOf("@", StringComparison.Ordinal) == email.Length - 1) ||
                (email.IndexOf("@", StringComparison.Ordinal) == 0))
            {
                throw new UnrealEmailException(email);
            }

            var unrealDate = DateTime.Today.YearsOld(birthday);
            if (unrealDate < 0 || unrealDate > 135)
            {
                throw new UnrealBirthdayException(birthday);
            }

            Name = name;
            Surname = surname;
            Email = email;
            Birthday = birthday;
        }
        public bool IsAdult => DateTime.Today.YearsOld(Birthday) >= 18;
        public bool IsBirthday => DateTime.Today.DayOfYear == Birthday.DayOfYear;
        public User(string name, string surname, string email) : this(name, surname, email, DateTime.Today) { }
        public User(string name, string surname, DateTime birthday) : this(name, surname, "not specified", birthday) { }
        public string ChineseSign
        {
            get
            {
                int sign = Birthday.Year % 12;
                switch (sign)
                {
                    case 0:
                        return "Рік Мавпи";

                    case 1:
                        return "Рік Півня";
                    case 2:
                        return "Рік Собаки";
                    case 3:
                        return "Рік Свині";
                    case 4:
                        return "Рік Пацюка";
                    case 5:
                        return "Рік Бика";
                    case 6:
                        return "Рік Тигра";
                    case 7:
                        return "Рік Кролика";
                    case 8:
                        return "Рік Дракона";
                    case 9:
                        return "Рік Змії";
                    case 10:
                        return "Рік Коня";
                    case 11:
                        return "Рік Кози";

                }
                return "";
            }
        }
        public string SunSign
        {
            get
            {
                var day = Birthday.Day;
                var month = Birthday.Month;
                if (day == -1)
                    return "Error Code -1";
                if (day > 21 && month == 12 || day < 20 && month == 1)
                    return "Козерог";
                else if (day > 19 && month == 1 || day < 19 && month == 2)
                    return "Водолій";
                else if (day > 18 && month == 2 || day < 21 && month == 3)
                    return "Риби";
                else if (day > 20 && month == 3 || day < 20 && month == 4)
                    return "Овен";
                else if (day > 19 && month == 4 || day < 21 && month == 5)
                    return "Телець";
                else if (day > 20 && month == 5 || day < 21 && month == 6)
                    return "Близнюки";
                else if (day > 20 && month == 6 || day < 23 && month == 7)
                    return "Рак";
                else if (day > 22 && month == 7 || day < 23 && month == 8)
                    return "Лев";
                else if (day > 22 && month == 8 || day < 23 && month == 9)
                    return "Діва";
                else if (day > 22 && month == 9 || day < 23 && month == 10)
                    return "Терези";
                else if (day > 22 && month == 10 || day < 22 && month == 11)
                    return "Скорпіон";
                else if (day > 21 && month == 11 || day < 22 && month == 12)
                    return "Стрілець";
                else
                    return "Козеріг";

            }
        }
        public void CopyUserFrom(User user)
        {
            Name = user.Name;
            Surname = user.Surname;
            Email = user.Email;
            Birthday = user.Birthday;
        }

        private void SaveUsersTo([NotNull] string filename)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Directory.CreateDirectory(Path.GetDirectoryName(filename) ?? throw new InvalidOperationException());
                Stream stream = new FileStream(path: filename,
                    mode: FileMode.Create,
                    access: FileAccess.Write,
                    share: FileShare.None);
                formatter.Serialize(serializationStream: stream, graph: this);
                stream.Close();
            }
            catch (IOException e)
            {
            }
        }

        private static User LoadFrom(string filename)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(filename,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);
                var user = (User)formatter.Deserialize(stream);
                stream.Close();
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async void LoadAllInto(List<User> users, Action action)
        {
            await Task.Run(() =>
            {
                if (!Directory.Exists(DataFilepath))
                {
                    Directory.CreateDirectory(DataFilepath);
                    users.AddRange(UsersCreator.CreateUsers(50));
                    SaveData(users);
                }
                else
                {
                    users.AddRange(Directory.EnumerateFiles(DataFilepath).Select(LoadFrom));
                }
                action();
            });
        }

        public static void SaveData([NotNull] List<User> users)
        {
            var i = 0;
            users.ForEach(delegate (User p)
            {
                p.SaveUsersTo(Path.Combine(DataFilepath, string.Format(UserFileTemplate, i++)));
            });
            string extraFile;
            while (File.Exists(extraFile = Path.Combine(DataFilepath, string.Format(UserFileTemplate, i++))))
                File.Delete(extraFile);
        }

        private static class UsersCreator
        {
            public static List<User> CreateUsers(int count)
            {
                var users = new List<User>();
                var random = new Random();
                for (var i = 0; i < count; ++i)
                {
                    var name = Names[random.Next(Names.Length)];
                    var surname = Surnames[random.Next(Surnames.Length)];
                    var email = $"{surname}.{name}@{EmailCompanies[random.Next(EmailCompanies.Length)]}";
                    var birthday = DateTime.Now.AddYears(-random.Next(10, 80)).AddDays(-random.Next(31)).AddMonths(-random.Next(12));

                    users.Add(new User(name, surname, email, birthday));
                }

                return users;
            }

            private static readonly string[] Names =
            {
                "Петя",
                "Абрам",
                "Коля",
                "Жора",
                "Славік",
                "Влад",
                "Віталій",
                "Карл",
                "Tрофим",
                "Міша",
                "Вова",
                "Лена",
                "Валя",
                "Валентин",
                "Степан",
                "Віка",
                "Віктор",
                "Ярослав",
                "Джон",
                "Майк",
                "Саша",
                "Леша",
                "Тім",
                "Тимофій",
                "Микола",
                "Наталя",
                "Андрій",
                "Кирило",
                "Зіна",
                "Земфіра",
                "Ітан",
                "Бен",
                "Пітер",
                "Лінда",
                "Іван",
                "Северус",
                "Ліза",
                "Гаррі",
                "Павло",
                "Юля",
                "Настя",
                "Юра",
                "Оля",
                "Люба",
                "Георг",
                "Лера",
                "Олег",
                "Аня",
                "Вася",
                "Аліна"
            };

            private static readonly string[] Surnames =
            {
                "Петя",
                "Абрам",
                "Коля",
                "Жора",
                "Славік",
                "Влад",
                "Віталій",
                "Карл",
                "Tрофим",
                "Міша",
                "Вова",
                "Лена",
                "Валя",
                "Валентин",
                "Степан",
                "Віка",
                "Віктор",
                "Ярослав",
                "Джон",
                "Майк",
                "Саша",
                "Леша",
                "Тім",
                "Тимофій",
                "Микола",
                "Наталя",
                "Андрій",
                "Кирило",
                "Зіна",
                "Земфіра",
                "Ітан",
                "Бен",
                "Пітер",
                "Лінда",
                "Іван",
                "Северус",
                "Ліза",
                "Гаррі",
                "Павло",
                "Юля",
                "Настя",
                "Юра",
                "Оля",
                "Люба",
                "Георг",
                "Лера",
                "Олег",
                "Аня",
                "Вася",
                "Аліна"
            };

            private static readonly string[] EmailCompanies =
            {
                "gmail.com",
                "i.ua",
                "yandex.ru",
                "mail.ru",
                "ukr.net",
                "ukma.edu.ua",
                "rambler.ru",
                "bigmir.net", 
                "meta.ua",
                "list.ru",
                "inbox.ru",
                "bk.ru"

            };
        }

        private class NotNullAttribute : Attribute
        {
        }
    }
}

