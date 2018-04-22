USE ucubot;
ALTER TABLE lesson_signal
DROP COLUMN user_id,
ADD student_id INT,
ADD CONSTRAINT FOREIGN KEY(student_id) REFERENCES student(id);