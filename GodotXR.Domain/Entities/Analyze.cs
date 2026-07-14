using System.ComponentModel.DataAnnotations.Schema;

namespace GodotXR.Domain.Entities
{
    public class Analyze : BaseEntity
    {
        public int ChildId { get; set; }

        // Mức độ chậm nói
        public SpeechLevel SpeechLevel { get; set; }

        // Chuẩn đoán
        public string? Diagnosis { get; set; }

        // Khó khăn của trẻ
        public string? Difficulties { get; set; }

        // Điểm mạnh
        public string? Strengths { get; set; }

        // Điểm yếu
        public string? Weaknesses { get; set; }

        // Khả năng giao tiếp
        public AssessmentLevel CommunicationAbility { get; set; }

        // Khả năng phát âm
        public AssessmentLevel PronunciationAbility { get; set; }

        // Khả năng hiểu ngôn ngữ
        public AssessmentLevel LanguageComprehension { get; set; }

        // Khả năng biểu đạt
        public AssessmentLevel LanguageExpression { get; set; }

        // Khả năng chú ý
        public AssessmentLevel AttentionLevel { get; set; }

        // Khả năng tương tác xã hội
        public AssessmentLevel SocialInteraction { get; set; }

        // Mục tiêu can thiệp
        public string? InterventionGoals { get; set; }

        // Khuyến nghị của chuyên gia
        public string? Recommendation { get; set; }

        // Ghi chú
        public string? Notes { get; set; }

        // Ngày đánh giá
        public DateTime AssessmentDate { get; set; }

        // Đánh giá tiếp theo
        public DateTime? NextAssessmentDate { get; set; }

        // Navigation Properties
        [ForeignKey("ChildId")]
        public ChildProfile Child { get; set; } = null!;

        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}