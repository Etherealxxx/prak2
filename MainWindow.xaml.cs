using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace diary
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Note> Notes = new List<Note>();

        private int SelectedNoteIndex;
        public MainWindow()
        {
            InitializeComponent();
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            Notes = Serializer.Deserialize();

            SelectedDate.SelectedDate = DateTime.Now;
        }

        private void NotesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach(Note note in Notes)
            {
                if (note.Title == e.AddedItems[0].ToString() && note.Date == SelectedDate.SelectedDate)
                {
                    TitleValue.Text = note.Title;
                    AboutValue.Text = note.About;
                    SelectedNoteIndex = Notes.IndexOf(note);
                }
            }
        }

        private void SetListBoxValues(DateTime date)
        {
            NotesListBox.Items.Clear();
            foreach (Note note in Notes)
            {
                if (note.Date == date)
                    NotesListBox.Items.Add(note.Title);
            }
        }

        private void SelectedDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearTextBoxes();
            NotesListBox.Items.Clear();
            SetListBoxValues((DateTime)SelectedDate.SelectedDate);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Notes.Add(new Note((DateTime)SelectedDate.SelectedDate, TitleValue.Text, AboutValue.Text));
            Serializer.Serialize(Notes);
            SetListBoxValues((DateTime)SelectedDate.SelectedDate);
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            Notes[SelectedNoteIndex].Title = TitleValue.Text;
            Notes[SelectedNoteIndex].About = AboutValue.Text;
            Serializer.Serialize(Notes);
            SetListBoxValues((DateTime)SelectedDate.SelectedDate);
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            ClearTextBoxes();
            Notes.Remove(Notes[SelectedNoteIndex]);
            Serializer.Serialize(Notes);
            SetListBoxValues((DateTime)SelectedDate.SelectedDate);
        }

        private void ClearTextBoxes()
        {
            TitleValue.Text = String.Empty;
            AboutValue.Text = String.Empty;
        }
    }

    public class Note
    {
        public DateTime Date;
        public string Title;
        public string About;

        public Note() { }

        public Note(DateTime date, string title, string about)
        {
            Date = date;
            Title = title;
            About = about;
        }
    }

    public static class Serializer
    {
        private static string FileName = "notes.json";

        public static void Serialize(List<Note> notes)
        {
            //FileStream createStream = File.Create(FileName);
            //System.Text.Json.JsonSerializer.Serialize(createStream, notes);
            string jsonString = JsonConvert.SerializeObject(notes); 
            File.WriteAllText(FileName, jsonString);
        }

        public static List<Note> Deserialize()
        {
            if (!File.Exists(FileName)) File.Create(FileName);

            string jsonString = File.ReadAllText(FileName);
            if (jsonString == "") return new List<Note>();
            List<Note> notes = JsonConvert.DeserializeObject<List<Note>>(jsonString);
            return notes;
        } 
    }
}
