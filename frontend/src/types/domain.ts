export type AssignmentStatus = 'Draft' | 'Published' | 'Closed';
export type SubmissionStatus = 'Pending' | 'Submitted' | 'Late' | 'Graded' | 'Rejected';

export interface SchoolClass {
  id: string;
  name: string;
  description?: string;
  academicYear: string;
  isActive: boolean;
  createdAt: string;
  studentCount?: number;
}

export interface Subject {
  id: string;
  name: string;
  code: string;
  description?: string;
  isActive: boolean;
  createdAt: string;
}

export interface TeacherSubject {
  id: string;
  teacherId: string;
  teacherName: string;
  subjectId: string;
  subjectName: string;
  subjectCode: string;
  classId: string;
  className: string;
  assignedAt: string;
  isActive: boolean;
}

export interface Assignment {
  id: string;
  title: string;
  description: string;
  dueDate: string;
  totalMarks: number;
  status: AssignmentStatus;
  teacherId: string;
  teacherName: string;
  subjectId: string;
  subjectName: string;
  subjectCode: string;
  classId: string;
  className: string;
  createdAt: string;
  updatedAt?: string;
}

export interface Submission {
  id: string;
  assignmentId: string;
  assignmentTitle: string;
  studentId: string;
  studentName: string;
  content?: string;
  filePath?: string;
  status: SubmissionStatus;
  submittedAt?: string;
  marksObtained?: number;
  feedback?: string;
  gradedAt?: string;
  dueDate: string;
  totalMarks: number;
}

export interface StudentAssignmentDetail {
  assignment: Assignment;
  submission: Submission | null;
}
