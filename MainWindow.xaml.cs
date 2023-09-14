using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ADO_EF.Data;
using ADO_EF.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ADO_EF
{
    public partial class MainWindow : Window
    {
        private DataContext dataContext;
        public ObservableCollection<Pair> Pairs { get; set; }
        public ObservableCollection<Department> DepartmentsView { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            dataContext = new();
            Pairs = new();
            this.DataContext = this;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            departmentsCountLabel.Content = dataContext.Departments.Count().ToString();
            managersCountLabel.Content = dataContext.Managers.Count().ToString();
            topChiefCountLabel.Content = dataContext.Managers.Where(manager => manager.IdChief == null) 
                                         .Count().ToString();       
                                                                    
            smallChiefCountLabel.Content = dataContext.Managers.Where(manager => manager.IdChief != null).Count().ToString();

          
            Guid itGuid = Guid.Parse(dataContext.Departments.Where(department => department.Name == "IT відділ")
                                     .Select(department => department.Id).First().ToString());
            itDepartCountLabel.Content = dataContext.Managers.Where(manager => manager.IdMainDep == itGuid || manager.IdSecDep == itGuid).Count().ToString();

            twoDepartCountLabel.Content = dataContext.Managers.Where(manager => manager.IdMainDep != null && manager.IdSecDep != null).Count().ToString();


            dataContext.Departments.Load();
            DepartmentsView = dataContext.Departments.Local.ToObservableCollection();
            departmentsList.ItemsSource = DepartmentsView;



        }

        private void UpdateCollection(IQueryable pairs)
        {
            Pairs.Clear();
            foreach (Pair pair in pairs)  // цикл-итератор запускает выполнение запроса
            {                             // с этого момента идёт запрос к БД
                Pairs.Add(pair);
            }
        }

        private void UpdateCollection(IEnumerable<Pair> pairs)
        {
            Pairs.Clear();
            foreach (Pair pair in pairs)
            {
                Pairs.Add(pair);
            }
        }


        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            // ФИО тех, кто работает в 'Бухгалтерии'
            IQueryable query = dataContext.Managers.Where(m => m.IdMainDep == Guid.Parse("131ef84b-f06e-494b-848f-bb4bc0604266"))
            .Select(m => new Pair { Key = m.Surname, Value = $"{m.Name[0]}. {m.Secname[0]}." });
            // Select - правило преобразования, на входе элемент предыдущей коллекции (m - manager),
            // а на выходе - результат лямбды
            // query - "правило" постройки запроса. Сам запрос не отправленый, и не получено результата

            // цикл-итератор foreach запускает выполнение запроса
            // с этого момента идёт запрос к БД
            UpdateCollection(query);

            // Особенности:
            // - LINQ запрос можно сохранить в переменной, сам запрос это "правило" и не
            //   инициализирует обращение к БД
            // - LINQ-to-Entity использует присоединённый режим, т.е. каждый запрос отправляется к БД,
            //   а не к "скаченой" коллекции
            // - Выполнение запроса выполняется шагами:
            //   1. вызов агрегатора (.Count(), .Max() и тд.)
            //   2. вызов явного преобразования (.ToList(), .ToArray(), и тд.)
            //   3. запуск цикла по итераванному запросу (foreach)

            // - Фильтрация (.Where) лучше задействовать с индексованными полями, и первичным ключом
            //   (который автоматически индексируется)
            // - Инструкция .Select это преобразователь, а не запуск запроса
        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
            // ФИО - отдел в котором работает, пропустить первые 3 и вывести первые 8
            IQueryable query = dataContext.Managers.Join(  // запрос с соединением таблиц
                    dataContext.Departments,
                    m => m.IdMainDep,  // внешний ключ левой таблицы
                    d => d.Id,  // первичный ключ правой таблицы
                    (m, d) => new Pair { Key = $"{m.Surname} {m.Name[0]}.{m.Secname[0]}.", Value = d.Name }
                    // selector - правило преобразования пары сущностей для которых зарегистрировано соединение (JOIN)
                )
                .Skip(3)   // пропустить первые 3 записи выборки
                .Take(8);  // получить первые 8 записей из выборки
            // Managers - левая таблица, Departments - правая таблица

            UpdateCollection(query);
        }

        private void Btn3_Click(object sender, RoutedEventArgs e)
        {
            // ФИО - ФИО шефа, отсортировать по ФИО работника
            IEnumerable<Pair> query = dataContext.Managers.Join(
                    dataContext.Managers,
                    m => m.IdChief,
                    chief => chief.Id,
                    (m, chief) => new Pair() { Key = $"{m.Surname} {m.Name[0]}.{m.Secname[0]}.", Value = $"{chief.Surname} {chief.Name[0]}.{chief.Secname[0]}." }
                )
                .ToList()  // запускает запрос и преобразовывает результат в коллекцию List<Pair>
                .OrderBy(pair => pair.Key);  // выполняется после Select, значит работает с Pair
                                             // но в данном случае, другой LINQ, который действует на коллекцию, а не на запрос SQL
            UpdateCollection(query);
        }

        private void Btn4_Click(object sender, RoutedEventArgs e)
        {
            // Дата создания записи - ФИО, первые 7 из последних по дате
            IQueryable query = dataContext.Managers
                .OrderByDescending(m => m.CreateDt)
                .Select(m => new Pair { Key = $"{m.CreateDt}", Value = $"{m.Surname} {m.Name[0]}.{m.Secname[0]}." })
                .Take(7);

            UpdateCollection(query);
        }

        private void Btn5_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Pair> query = dataContext.Managers.Join(
                    dataContext.Departments,
                    m => m.IdSecDep,
                    d => d.Id,
                    (m, d) => new Pair() { Key = $"{m.Surname} {m.Name[0]}.{m.Secname[0]}.", Value = d.Name }
                ).OrderBy(pair => pair.Value);

            UpdateCollection(query);
        }
        #region Генератор - збільшується при кожному зверненні
        private int _N;
        public int N { get => _N++; set => _N = value; }
        #endregion
        private void  btn6_Click(object sender, RoutedEventArgs e)
        {
            /*Вывусты порядковый номер відділу -- назву відділу
             */
            N = 1;
            var query = dataContext
                .Departments
                .Select(d => new Pair()
                {
                    Key = (N).ToString(),
                    Value = d.Name
                }) ;

            Pairs.Clear();
            foreach(var pair in query)
            {
                Pairs.Add(pair);
            }
             
        }

        private void Btn7_Click(object sender, RoutedEventArgs e)
        {
            N = 1;
            var query = dataContext
                .Departments
                .OrderBy(d => d.Name)
                .AsEnumerable()
                .Select(d => new Pair()
                {
                    Key = (N).ToString(),
                    Value = d.Name
                });

            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }
        }

        private void Btn8_Click(object sender, RoutedEventArgs e)
        {
            var query = dataContext
                .Departments
                .GroupJoin(
                    dataContext.Managers,
                    d => d.Id,
                    m => m.IdMainDep,
                    (d, m) => new Pair
                    {
                        Key = d.Name,
                        Value = m.Count().ToString()
                    }
                );
            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }


        }

        private void btn9_Click(object sender, RoutedEventArgs e)
        {
            var query = dataContext.Managers  // as chef
                .GroupJoin(
                    dataContext.Managers, // as sub
                    chef => chef.Id,
                    sub => sub.IdChief,
                    (chef, subs) => new Pair()
                    {
                        Key = $"{chef.Surname} {chef.Name[0]}. {chef.Secname[0]}.",
                        Value = subs.Count().ToString()
                    }
                )
                .Where(p => Convert.ToInt32(p.Value) > 0);

            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }
        }

        private void btn10_Click(object sender, RoutedEventArgs e)
        {
            /* Знайти однофамільців (згрупувати та подивитись де кількість 
            * більше ніж 1)
            */
            var query = dataContext.Managers
                .GroupBy(m => m.Surname)  // 
                .AsEnumerable()
                .Select(group => new Pair
                {
                    Key = group.Key,  // group.Key  --- m => m.Surname
                    Value = group.Count().ToString()
                })
                .Where(p => Convert.ToInt32(p.Value) > 1);

            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }
        }

        /* Д.З. Засобами LINQ на основі створеної БД реалізувати запити
         * Назва відділу -- кількість сумісників (SecDep)
         * Запит з однофамільцями переробити з нумерацією
         *  1. Андріяш
         *  2. Лешків
         * Вивести трьох співробітників з найбільшою кількістю підлеглих 
         *  к-сть підлеглих --- П.І.Б.
         */

        private void btn11_Click(object sender, RoutedEventArgs e)
        {
            var query = dataContext
                .Departments
                .GroupJoin(
                    dataContext.Managers,
                    d => d.Id,
                    m => m.IdSecDep,
                    (d, m) => new Pair
                    {
                        Key = d.Name,
                        Value = m.Count().ToString()
                    }
                );
            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }


        }

        private void Nav_Click(object sender, RoutedEventArgs e)
        {
            // робота з навігаційними властивостями:
            // вивести: співробітник - назва відділу
            var query = dataContext
                .Managers
                .Include(m => m.MainDep)
                .Select(m => new Pair
                {
                    Key = m.Surname,
                    Value = m.MainDep.Name
                });
                
            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }
        }

        private void Nav2_Click(object sender, RoutedEventArgs e)
        {
            var query = dataContext
                .Managers
                .Include(m => m.SecDep)
                .Select(m => new Pair
                {
                    Key = m.Surname,
                    Value = m.SecDep.Name == null ? "\t-" : m.SecDep.Name
                });
                

            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(sender is ListViewItem item)
            {
                if(item.Content is Department department)
                {
                    CrudDepartmentWindow dialog = new()
                    {
                        Department = department
                    };

                    if (dialog.ShowDialog() ?? false)
                    {
                        if (dialog.Department != null)
                        {
                            var dep = dataContext.Departments.Find(department.Id);
                            if (dep != null)
                            {
                                dep.Name = department.Name;
                            }
                            dataContext.SaveChanges();
                            dataContext.Departments.Local.Reset();

                            //оновлення коллекції (savaChanges выдает исключение)
                            //int index = DepartmentsView.IndexOf(department);
                            //DepartmentsView.Remove(department);
                            //DepartmentsView.Insert(index, department);
                        }
                        else
                        {
                            dataContext.Departments.Remove(department);
                            dataContext.SaveChanges();
                        }
                    }
                }
            }
            
            
        }

        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            // Create - створення нового відділу
            // спочатку створюємо новий об'єкт (Entity)
            Data.Entity.Department newDepartment = new()
            {
                Id = Guid.NewGuid(),
                Name = null!
            };
            // заповнюємо його - викликаємо вікно-діалог
            CrudDepartmentWindow dialog = new()
            {
                Department = newDepartment
            };
            if (dialog.ShowDialog() ?? false)  // Save or Delete pressed
            {
                // Після заповнення - додаємо об'єкт до контексту даних
                dataContext.Departments.Add(newDepartment);
                // зберігаємо контекст
                //dataContext.SaveChanges();
            }
        }

        private void Nav3_Click(object sender, RoutedEventArgs e)
        {
            //Inverse Navigations Props - зворотні навігаціййні властивості
            //Завдання: Вивести відділ - кількість співробітників

            var query = dataContext
                .Departments
                //.Where(d => d.DeleteDt == null)
                .Select(d => new Pair
                {
                    Key = d.Name,
                    Value = d.MainManagers.Count().ToString()
                });

            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }
        }

        private void Nav4_Click(object sender, RoutedEventArgs e)
        {
            //Inverse Navigations Props - зворотні навігаціййні властивості
            //Завдання: Вивести відділ - кількість співробітників за сумісництвом

            var query = dataContext
                .Managers
                .Include(m => m.SubManagers)
                //.Where(d => d.DeleteDt == null)
                .Select(d => new Pair
                {
                    Key = d.Name,
                    Value = d.SubManagers.Count().ToString()
                });

            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }
        }
    }

    public class Pair
    {
        public string Key { get; set; } = null!;
        public string? Value { get; set; }
    }
}