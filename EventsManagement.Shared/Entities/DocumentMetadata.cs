namespace EventsManagement.Shared.Entities
{
    /// <summary>
    /// کلاس مستندات
    /// </summary>
    public class DocumentMetadata : BaseEntity
    {
        /// <summary>
        /// نام فایل
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// نوع فایل
        /// </summary>
        public string FileType { get; set; } // "Image", "Video", "PDF", "Audio"

        /// <summary>
        /// حجم فایل به بایت
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// مسیر ذخیره‌سازی فایل
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// شناسه رویداد (اختیاری)
        /// </summary>
        public int? EventId { get; set; }

        /// <summary>
        /// شناسه وظیفه (اختیاری)
        /// </summary>
        public int? TaskId { get; set; }

        /// <summary>
        /// شناسه پاسخ (اختیاری)
        /// </summary>
        public int? TaskReplyId { get; set; }

        /// <summary>
        /// شناسه کاربر آپلود‌کننده
        /// </summary>
        public string UploadedBy { get; set; }

        /// <summary>
        /// شرح یا نام نمایش
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// آیا عمومی است
        /// </summary>
        public bool IsPublic { get; set; } = false;
    }
}
