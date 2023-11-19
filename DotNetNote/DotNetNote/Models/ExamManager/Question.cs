namespace DotNetNote.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Point { get; set; }

        public int OrderNumber { get; set; }

        private IList<Choice> _choices = new List<Choice>();
        public IList<Choice> Choices
        {
            get { return _choices; }
            set { _choices = value;}
        }

        public void AddChoice(Choice choice)
        {
            _choices.Add(choice);
            choice.Question = this; 
        }
    }
}

// Exams_Questions


//CREATE TABLE users(
// id int(10) auto_increment primary key,
//  username VARCHAR(45) NOT NULL,
//  password VARCHAR(45) NOT NULL,
//  enabled TINYINT NOT NULL DEFAULT 1
//);

//CREATE TABLE questions(
//    id int(10) auto_increment primary key,
//    question varchar(800) NOT NULL,
//    right_option int (10) NOT NULL references options(id)
//);

//CREATE TABLE options(
//    id int(10) auto_increment primary key,
//    question_id int (10) NOT NULL references questions(id),
//    `option` varchar(150) NOT NULL
//);

//CREATE TABLE exam_details(
//    id int(10) auto_increment primary key,
//    username varchar(45) NOT NULL references users(username),
//    date_of_exam date not null,
//    exam_result varchar(10) NOT NULL, -- PASS/FAIL
//    exam_score int (10) NOT NULL,      -- e.g. 40 
//    no_of_questions int (10) NOT NULL  -- total no.of questions in the test
// );

//CREATE TABLE user_answers(
//    id int(10) auto_increment primary key,
//    userId int (10) NOT NULL references users(id),
//    question_id int (10) NOT NULL references questions(id),
//    answer int (10) NOT NULL references options(id)
//);


// 또 다른 모양
//create table questions(
//    question_id number primary key,
//    question_text text
//);
//create table questions_choices(
//    question_id number references questions(question_id),
//    choice_number number,
//    choice_text text,
//    primary key(question_id, choice_number)
//);
//create table answers(
//    answer_id number primary key,
//    user_id number references users(user_id),
//    question_id number,
//    choice_number number,
//    foreign key (question_id, choice_number) references questions_choices(question_id, choice_number)
//);
