using Talent.Common.Contracts;
using Talent.Common.Models;
using Talent.Services.Profile.Domain.Contracts;
using Talent.Services.Profile.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Talent.Services.Profile.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Talent.Common.Security;

namespace Talent.Services.Profile.Domain.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserAppContext _userAppContext;
        IRepository<UserLanguage> _userLanguageRepository;
        IRepository<User> _userRepository;
        IRepository<UserSkill> _userSkillRepository;
        IRepository<UserExperience> _userExperienceRepository;
        IRepository<Employer> _employerRepository;
        IRepository<Job> _jobRepository;
        IRepository<Recruiter> _recruiterRepository;
        IFileService _fileService;


        public ProfileService(IUserAppContext userAppContext,
                              IRepository<UserLanguage> userLanguageRepository,
                              IRepository<User> userRepository,
                              IRepository<UserSkill> userSkillRepository,
                              IRepository<UserExperience> userExperienceRepository,
                              IRepository<Employer> employerRepository,
                              IRepository<Job> jobRepository,
                              IRepository<Recruiter> recruiterRepository,
                              IFileService fileService)
        {
            _userAppContext = userAppContext;
            _userLanguageRepository = userLanguageRepository;
            _userRepository = userRepository;
            _userSkillRepository = userSkillRepository;
            _userExperienceRepository = userExperienceRepository;
            _employerRepository = employerRepository;
            _jobRepository = jobRepository;
            _recruiterRepository = recruiterRepository;
            _fileService = fileService;
        }

        public async Task<bool> AddNewLanguage(AddLanguageViewModel language)
        {
            language.CurrentUserId = _userAppContext.CurrentUserId;
            if (language.CurrentUserId != null)
            {
                var objectId = ObjectId.GenerateNewId().ToString();

                var newUserLanguage = new UserLanguage
                {
                    Id = objectId,
                    UserId = language.CurrentUserId,
                    Language = language.Language,
                    LanguageLevel = language.LanguageLevel,
                    IsDeleted = false
                };

                await _userLanguageRepository.Add(newUserLanguage);
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateLanguage(AddLanguageViewModel language)

        {
            language.CurrentUserId = _userAppContext.CurrentUserId;

            if (language.CurrentUserId != null && language.Id != null)
            {
                UserLanguage listLanguage = await _userLanguageRepository.GetByIdAsync(language.Id);
                listLanguage.Language = language.Language;
                listLanguage.LanguageLevel = language.LanguageLevel;


                await _userLanguageRepository.Update(listLanguage);
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteLanguage(AddLanguageViewModel language)
        {
            if (language.Id != null)
            {
                UserLanguage listLanguage = await _userLanguageRepository.GetByIdAsync(language.Id);
                listLanguage.IsDeleted = true;
                await _userLanguageRepository.Delete(listLanguage);
                return true;
            }
            return false;

        }
        public async Task<bool> AddNewSkill(AddSkillViewModel skill)
        {
            skill.Id = _userAppContext.CurrentUserId;
            if (skill.Id != null)
            {
                var objectId = ObjectId.GenerateNewId().ToString();

                var newSkill = new UserSkill
                {
                    Id = objectId,
                    UserId = skill.Id,
                    Skill = skill.Name,
                    ExperienceLevel = skill.Level,
                    IsDeleted = false

                };
                await _userSkillRepository.Add(newSkill);
                return true;
            }
            return false;


        }
        public async Task<bool> UpdateSkill(AddSkillViewModel skill)

        {
            if (skill.Id != null)
            {
                UserSkill existingSkill = await _userSkillRepository.GetByIdAsync(skill.Id);
                existingSkill.Skill = skill.Name;
                existingSkill.ExperienceLevel = skill.Level;


                await _userSkillRepository.Update(existingSkill);
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteSkill(AddSkillViewModel skill)
        {
            if (skill.Id != null)
            {

                UserSkill existingSkill = await _userSkillRepository.GetByIdAsync(skill.Id);
                await _userSkillRepository.Delete(existingSkill);
                return true;
            }
            return false;
        }
        public async Task<bool> AddNewExperience(ExperienceViewModel experience)
        {
            experience.Id = _userAppContext.CurrentUserId;

            if (experience.Id != null)
            {
                var objectId = ObjectId.GenerateNewId().ToString();

                var newUserExperience = new UserExperience
                {
                    Id = objectId,
                    UserId = experience.Id,
                    Company = experience.Company,
                    Position = experience.Position,
                    Responsibilities = experience.Responsibilities,
                    Start = experience.Start,
                    End = experience.End,
                    IsDeleted = false

                };
                await _userExperienceRepository.Add(newUserExperience);
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateExperience(ExperienceViewModel experience)
        {
            experience.UserId = _userAppContext.CurrentUserId;

            if (experience.UserId != null && experience.Id != null)
            {
                UserExperience existingExperience = await _userExperienceRepository.GetByIdAsync(experience.Id);

                existingExperience.Company = experience.Company;
                existingExperience.Position = experience.Position;
                existingExperience.Responsibilities = experience.Responsibilities;
                existingExperience.Start = experience.Start;
                existingExperience.End = experience.End;

                await _userExperienceRepository.Update(existingExperience);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteExperience(ExperienceViewModel experience)
        {
            if (experience.Id != null)
            {

                UserExperience existingExperience = await _userExperienceRepository.GetByIdAsync(experience.Id);
                await _userExperienceRepository.Delete(existingExperience);
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateTalentPhoto(string talentId, IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            List<string> acceptedExtensions = new List<string> { ".jpg", ".png", ".gif", ".jpeg" };

            if (fileExtension != null && !acceptedExtensions.Contains(fileExtension.ToLower()))
            {
                return false;
            }

            var profile = (await _userRepository.Get(x => x.Id == talentId)).SingleOrDefault();

            if (profile == null)
            {
                return false;
            }

            var newFileName = await _fileService.SaveFile(file, FileType.ProfilePhoto);

            if (!string.IsNullOrWhiteSpace(newFileName))
            {
                var oldFileName = profile.ProfilePhoto;

                if (!string.IsNullOrWhiteSpace(oldFileName))
                {
                    await _fileService.DeleteFile(oldFileName, FileType.ProfilePhoto);
                }

                profile.ProfilePhoto = newFileName;
                profile.ProfilePhotoUrl = await _fileService.GetFileURL(newFileName, FileType.ProfilePhoto);

                await _userRepository.Update(profile);
                return true;
            }

            return false;

        }
        public async Task<TalentProfileViewModel> GetTalentProfile(string Id)
        {
            User profile = null;
            profile = await _userRepository.GetByIdAsync(Id);


            if (profile != null)
            {

                var languages = await _userLanguageRepository.Get(x => x.UserId == profile.Id && !x.IsDeleted);
                var newUserLanguages = languages.Select(x => ViewModelFromLanguage(x)).ToList();

                var skills = await _userSkillRepository.Get(x => x.UserId == profile.Id && !x.IsDeleted);
                var newUserSkills = skills.Select(x => ViewModelFromSkill(x)).ToList();

                var experiences = await _userExperienceRepository.Get(x => x.UserId == profile.Id && !x.IsDeleted);
                var newUserExperiences = experiences.Select(x => ViewModelFromExperience(x)).ToList();



                var result = new TalentProfileViewModel
                {
                    Id = profile.Id,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    Email = profile.Email,
                    Phone = profile.Phone,
                    Address = profile.Address,
                    Nationality = profile.Nationality,
                    VisaStatus = profile.VisaStatus,
                    VisaExpiryDate = profile.VisaExpiryDate,
                    Summary = profile.Summary,
                    Description = profile.Description,
                    Languages = newUserLanguages,
                    Skills = newUserSkills,
                    Experience = newUserExperiences,
                    ProfilePhoto = profile.ProfilePhoto,
                    ProfilePhotoUrl = profile.ProfilePhotoUrl,
                    JobSeekingStatus = profile.JobSeekingStatus,
                    LinkedAccounts = profile.LinkedAccounts

                };
                return result;
            }

            return null;
        }

        public async Task<bool> UpdateTalentProfile(TalentProfileViewModel model, string updaterId)
        {
            try
            {
                if (model.Id != null)
                {
                    User existingUser = (await _userRepository.GetByIdAsync(model.Id));
                    existingUser.FirstName = model.FirstName;
                    existingUser.LastName = model.LastName;
                    existingUser.Email = model.Email;
                    existingUser.Phone = model.Phone;
                    existingUser.Address = model.Address;
                    existingUser.Nationality = model.Nationality;
                    existingUser.VisaStatus = model.VisaStatus;
                    existingUser.VisaExpiryDate = model.VisaExpiryDate;
                    existingUser.ProfilePhoto = model.ProfilePhoto;
                    existingUser.ProfilePhotoUrl = model.ProfilePhotoUrl;
                    existingUser.JobSeekingStatus = model.JobSeekingStatus;
                    existingUser.LinkedAccounts = model.LinkedAccounts;
                    existingUser.Summary = model.Summary;
                    existingUser.Description = model.Description;

                    existingUser.UpdatedBy = updaterId;
                    existingUser.UpdatedOn = DateTime.Now;

                    await _userRepository.Update(existingUser);
                    return true;
                }
                return false;
            }
            catch (MongoException e)
            {
                return false;
            }
        }

        public async Task<EmployerProfileViewModel> GetEmployerProfile(string Id, string role)
        {

            Employer profile = null;
            switch (role)
            {
                case "employer":
                    profile = (await _employerRepository.GetByIdAsync(Id));
                    break;
                case "recruiter":
                    profile = (await _recruiterRepository.GetByIdAsync(Id));
                    break;
            }

            var videoUrl = "";

            if (profile != null)
            {
                videoUrl = string.IsNullOrWhiteSpace(profile.VideoName)
                          ? ""
                          : await _fileService.GetFileURL(profile.VideoName, FileType.UserVideo);

                var skills = profile.Skills.Select(x => ViewModelFromSkill(x)).ToList();

                var result = new EmployerProfileViewModel
                {
                    Id = profile.Id,
                    CompanyContact = profile.CompanyContact,
                    PrimaryContact = profile.PrimaryContact,
                    Skills = skills,
                    ProfilePhoto = profile.ProfilePhoto,
                    ProfilePhotoUrl = profile.ProfilePhotoUrl,
                    VideoName = profile.VideoName,
                    VideoUrl = videoUrl,
                    DisplayProfile = profile.DisplayProfile,
                };
                return result;
            }

            return null;
        }

        public async Task<bool> UpdateEmployerProfile(EmployerProfileViewModel employer, string updaterId, string role)
        {
            try
            {
                if (employer.Id != null)
                {
                    switch (role)
                    {
                        case "employer":
                            Employer existingEmployer = (await _employerRepository.GetByIdAsync(employer.Id));
                            existingEmployer.CompanyContact = employer.CompanyContact;
                            existingEmployer.PrimaryContact = employer.PrimaryContact;
                            existingEmployer.ProfilePhoto = employer.ProfilePhoto;
                            existingEmployer.ProfilePhotoUrl = employer.ProfilePhotoUrl;
                            existingEmployer.DisplayProfile = employer.DisplayProfile;
                            existingEmployer.UpdatedBy = updaterId;
                            existingEmployer.UpdatedOn = DateTime.Now;

                            var newSkills = new List<UserSkill>();
                            foreach (var item in employer.Skills)
                            {
                                var skill = existingEmployer.Skills.SingleOrDefault(x => x.Id == item.Id);
                                if (skill == null)
                                {
                                    skill = new UserSkill
                                    {
                                        Id = ObjectId.GenerateNewId().ToString(),
                                        IsDeleted = false
                                    };
                                }
                                UpdateSkillFromView(item, skill);
                                newSkills.Add(skill);
                            }
                            existingEmployer.Skills = newSkills;

                            await _employerRepository.Update(existingEmployer);
                            break;

                        case "recruiter":
                            Recruiter existingRecruiter = (await _recruiterRepository.GetByIdAsync(employer.Id));
                            existingRecruiter.CompanyContact = employer.CompanyContact;
                            existingRecruiter.PrimaryContact = employer.PrimaryContact;
                            existingRecruiter.ProfilePhoto = employer.ProfilePhoto;
                            existingRecruiter.ProfilePhotoUrl = employer.ProfilePhotoUrl;
                            existingRecruiter.DisplayProfile = employer.DisplayProfile;
                            existingRecruiter.UpdatedBy = updaterId;
                            existingRecruiter.UpdatedOn = DateTime.Now;

                            var newRSkills = new List<UserSkill>();
                            foreach (var item in employer.Skills)
                            {
                                var skill = existingRecruiter.Skills.SingleOrDefault(x => x.Id == item.Id);
                                if (skill == null)
                                {
                                    skill = new UserSkill
                                    {
                                        Id = ObjectId.GenerateNewId().ToString(),
                                        IsDeleted = false
                                    };
                                }
                                UpdateSkillFromView(item, skill);
                                newRSkills.Add(skill);
                            }
                            existingRecruiter.Skills = newRSkills;
                            await _recruiterRepository.Update(existingRecruiter);

                            break;
                    }
                    return true;
                }
                return false;
            }
            catch (MongoException e)
            {
                return false;
            }
        }

        public async Task<bool> UpdateEmployerPhoto(string employerId, IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            List<string> acceptedExtensions = new List<string> { ".jpg", ".png", ".gif", ".jpeg" };

            if (fileExtension != null && !acceptedExtensions.Contains(fileExtension.ToLower()))
            {
                return false;
            }

            var profile = (await _employerRepository.Get(x => x.Id == employerId)).SingleOrDefault();

            if (profile == null)
            {
                return false;
            }

            var newFileName = await _fileService.SaveFile(file, FileType.ProfilePhoto);

            if (!string.IsNullOrWhiteSpace(newFileName))
            {
                var oldFileName = profile.ProfilePhoto;

                if (!string.IsNullOrWhiteSpace(oldFileName))
                {
                    await _fileService.DeleteFile(oldFileName, FileType.ProfilePhoto);
                }

                profile.ProfilePhoto = newFileName;
                profile.ProfilePhotoUrl = await _fileService.GetFileURL(newFileName, FileType.ProfilePhoto);

                await _employerRepository.Update(profile);
                return true;
            }

            return false;

        }

        public async Task<bool> AddEmployerVideo(string employerId, IFormFile file)
        {
            //Your code here;
            throw new NotImplementedException();
        }


        public async Task<bool> AddTalentVideo(string talentId, IFormFile file)
        {
            //Your code here;
            throw new NotImplementedException();

        }

        public async Task<bool> RemoveTalentVideo(string talentId, string videoName)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateTalentCV(string talentId, IFormFile file)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> GetTalentSuggestionIds(string employerOrJobId, bool forJob, int position, int increment)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TalentSnapshotViewModel>> GetTalentSnapshotList(string employerOrJobId, bool forJob, int position, int increment)
        {
            var profile = await _employerRepository.GetByIdAsync(employerOrJobId);
            var talents = _userRepository.GetQueryable().Skip(position).Take(increment);

            if (profile != null)
            {
                var newTalentProfiles = new List<TalentSnapshotViewModel>();
                foreach (var talent in talents)
                {


                    string name = talent.FirstName + " " + talent.LastName;
                    var videoUrl = _fileService.GetFileURL(talent.VideoName, FileType.UserVideo);
                    List<string> skills = talent.Skills.Select(x => x.Skill).ToList();

                    UserExperience latest = talent.Experience.OrderByDescending(x => x.End).FirstOrDefault();
                    string level, employment;
                    if (latest != null)
                    {
                        level = latest.Position;
                        employment = latest.Company;
                    }
                    else
                    {
                        level = "Not available";
                        employment = "Not available";
                    }
                    var talentSnapshot = new TalentSnapshotViewModel
                    {
                        Id = talent.Id,
                        Level = level,
                        CurrentEmployment = employment,
                        Name = name,
                        PhotoId = talent.ProfilePhotoUrl,
                        Skills = skills,
                        CVUrl = talent.CvName,
                        //VideoUrl = videoUrl,
                        Summary = talent.Summary,
                        Visa = talent.VisaStatus,
                        LinkedIn = talent.LinkedAccounts.LinkedIn,
                        GitHub = talent.LinkedAccounts.Github,
                    };

                    newTalentProfiles.Add(talentSnapshot);
                }

                return newTalentProfiles;
            }
            return null;

        }

    

        public async Task<IEnumerable<TalentSnapshotViewModel>> GetTalentSnapshotList(IEnumerable<string> ids)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        #region TalentMatching

        public async Task<IEnumerable<TalentSuggestionViewModel>> GetFullTalentList()
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public IEnumerable<TalentMatchingEmployerViewModel> GetEmployerList()
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TalentMatchingEmployerViewModel>> GetEmployerListByFilterAsync(SearchCompanyModel model)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TalentSuggestionViewModel>> GetTalentListByFilterAsync(SearchTalentModel model)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TalentSuggestion>> GetSuggestionList(string employerOrJobId, bool forJob, string recruiterId)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<bool> AddTalentSuggestions(AddTalentSuggestionList selectedTalents)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        #endregion

        #region Conversion Methods

        #region Update from View

        protected void UpdateSkillFromView(AddSkillViewModel model, UserSkill original)
        {
            original.ExperienceLevel = model.Level;
            original.Skill = model.Name;
        }

        protected void UpdateLanguageFromView(AddLanguageViewModel model, UserLanguage original)
        {
            original.LanguageLevel = model.LanguageLevel;
            original.Language = model.Language;
        }

        protected void UpdateExperienceFromView(ExperienceViewModel model, UserExperience original)
        {
            original.Company = model.Company;
            original.Position = model.Position;
            original.Start = model.Start;
            original.End = model.End;
            original.Responsibilities = model.Responsibilities;
        }
        #endregion

        #region Build Views from Model

        protected AddSkillViewModel ViewModelFromSkill(UserSkill skill)
        {
            return new AddSkillViewModel
            {
                Id = skill.Id,
                Level = skill.ExperienceLevel,
                Name = skill.Skill
            };
        }

        protected AddLanguageViewModel ViewModelFromLanguage(UserLanguage language)
        {
            return new AddLanguageViewModel
            {
                Id = language.Id,
                LanguageLevel = language.LanguageLevel,
                Language = language.Language
                
            };
        }

        protected ExperienceViewModel ViewModelFromExperience(UserExperience experience)
        {
            return new ExperienceViewModel
            {
                Id = experience.Id,
                UserId = experience.UserId,
                Company = experience.Company,
                Position = experience.Position,
                Start = experience.Start,
                End = experience.End,
                Responsibilities = experience.Responsibilities
            };
        }

        



        #endregion

        #endregion

        #region ManageClients

        public async Task<IEnumerable<ClientViewModel>> GetClientListAsync(string recruiterId)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<ClientViewModel> ConvertToClientsViewAsync(Client client, string recruiterId)
        {
            //Your code here;
            throw new NotImplementedException();
        }
         
        public async Task<int> GetTotalTalentsForClient(string clientId, string recruiterId)
        {
            //Your code here;
            throw new NotImplementedException();

        }

        public async Task<Employer> GetEmployer(string employerId)
        {
            return await _employerRepository.GetByIdAsync(employerId);
        }
        #endregion

    }
}
