using Cagnaz.Family.Application.Repositorys;
using Cagnaz.Family.Domain.DTOModels.FamilyModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Responder.Models;

namespace Cagnaz.Family.WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class FamilyController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public FamilyController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        [HttpPost("AddFamily")]
        public async Task<IActionResult> AddFamily(CreateFamilyModel family)=>
            Ok(new Response<Guid>(_Data:await _uow.FamilyRepository.AddFamily(family),_Message:"Aile başarıyla eklendi"));
           
        [HttpGet("GetFamilyByUserID/{usrID}")]
        public async Task<IActionResult> GetFamilyByUserID(Guid usrID)=>
            Ok(new Response<FamilyListModel?>(_Data:await _uow.FamilyRepository.GetFamilyByUserID(usrID),_Message:"Kullanıcının aile bilgileri getirildi"));
        [HttpPost("JoinFamily")]
        public async Task<IActionResult> JoinFamily(JoinFamilyModel family){
            await _uow.FamilyRepository.JoinFamily(family);
            return Ok(new Response<string>(_Data:null,_Message:"Aileye başarıyla katıldınız"));
        }
        [HttpGet("LeaveFamily/{memberID}")]
        public async Task<IActionResult> LeaveFamily(Guid memberID){
            await _uow.FamilyRepository.LeaveFamily(memberID);
            return Ok(new Response<string>(_Data:null,_Message:"Aileden başarıyla ayrıldınız"));
        }
        [HttpPost("AddNotUserFamilyMember")]
        public async Task<IActionResult> AddNotUserFamilyMember(FamilyMemberModel model){
            await _uow.FamilyRepository.AddNotUserFamilyMember(model);
            return Ok(new Response<string>(_Data:null,_Message:"Aileye başarıyla üye eklendi"));
        }
        [HttpPut("UpdateFamilyMember")]
        public async Task<IActionResult> UpdateFamilyMember(UpdateFamilyMemberModel model){
            await _uow.FamilyRepository.UpdateFamilyMember(model);
            return Ok(new Response<string>(_Data:null,_Message:"Aile üyesi başarıyla güncellendi"));
        }
    }
}
