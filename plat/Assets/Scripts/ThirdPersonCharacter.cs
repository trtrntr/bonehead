using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class ThirdPersonCharacter : MonoBehaviour
    {
        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField] float stationaryTurnSpeed = 180;
        [SerializeField] float jumpPower = 8f;
        [SerializeField] float runCycleLegOffset = 0.2f;
        [SerializeField] float moveSpeedMultiplier = 1f;
        [SerializeField] float animSpeedMultiplier = 1f;
        [SerializeField] float groundCheckDistance = 0.1f;

        new Rigidbody rigidbody;
        Vector3 groundNormal;
        Vector3 capsuleCenter;
        Animator animator;
        CapsuleCollider capsuleCol;

        float origGroundCheckDistance;
        float turnAmount;
        float forwardAmount;
        float capsuleHeight;
        const float k_Half = 0.5f;

        bool isCrouching;
        bool isGrounded;

        void Start()
        {
            animator = GetComponent<Animator>();
            rigidbody = GetComponent<Rigidbody>();
            capsuleCol = GetComponent<CapsuleCollider>();

            capsuleHeight = capsuleCol.height;
            capsuleCenter = capsuleCol.center;

            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            origGroundCheckDistance = groundCheckDistance;
        }

        public void Move(Vector3 move, bool crouch, bool jump)
        {
            if (move.magnitude > 1f)
            {
                move.Normalize();
            }
            move = transform.InverseTransformDirection(move);
            CheckGroundStatus();
            move = Vector3.ProjectOnPlane(move, groundNormal);
            turnAmount = Mathf.Atan2(move.x, move.z);
            forwardAmount = move.z;

            ApplyExtraTurnRotation();

            if (isGrounded)
            {
                HandleGroundedMovement(crouch, jump);
            }
            else
            {
                HandleAirborneMovement();
            }

            ScaleCapsuleForCrouching(crouch);
            PreventStandingInLowHeadroom();

            UpdateAnimator(move);
        }

        void ScaleCapsuleForCrouching(bool crouch)
        {
            if (isGrounded && crouch)
            {
                if (isCrouching)
                {
                    return;
                }
                capsuleCol.height = capsuleCol.height / 2f;
                capsuleCol.center = capsuleCol.center / 2f;
                isCrouching = true;
            }
            else
            {
                Ray crouchRay = new(rigidbody.position + Vector3.up * capsuleCol.radius * k_Half, Vector3.up);
                float crouchRayLength = capsuleHeight - capsuleCol.radius * k_Half;
                if (Physics.SphereCast(crouchRay, capsuleCol.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    isCrouching = true;
                    return;
                }
                capsuleCol.height = capsuleHeight;
                capsuleCol.center = capsuleCenter;
                isCrouching = false;
            }
        }

        void PreventStandingInLowHeadroom()
        {
            if (!isCrouching)
            {
                Ray crouchRay = new Ray(rigidbody.position + Vector3.up * capsuleCol.radius * k_Half, Vector3.up);
                float crouchRayLength = capsuleHeight - capsuleCol.radius * k_Half;
                if (Physics.SphereCast(crouchRay, capsuleCol.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    isCrouching = true;
                }
            }
        }

        void UpdateAnimator(Vector3 move)
        {
            animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
            animator.SetBool("Crouch", isCrouching);
            animator.SetBool("OnGround", isGrounded);
            if (!isGrounded)
            {
                animator.SetFloat("Jump", rigidbody.velocity.y);
            }

            float runCycle = Mathf.Repeat(
                    animator.GetCurrentAnimatorStateInfo(0).normalizedTime + runCycleLegOffset, 1);

            float jumpLeg = (runCycle < k_Half ? 1 : -1) * forwardAmount;
            if (isGrounded)
            {
                animator.SetFloat("JumpLeg", jumpLeg);
            }

            if (isGrounded && move.magnitude > 0)
            {
                animator.speed = animSpeedMultiplier;
            }

            else {; }
        }
        void HandleAirborneMovement() {; }

        void HandleGroundedMovement(bool crouch, bool jump)
        {

            if (jump && !crouch && animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
            {

                rigidbody.velocity = new Vector3(rigidbody.velocity.x, jumpPower, rigidbody.velocity.z);
                isGrounded = false;
                animator.applyRootMotion = false;
                groundCheckDistance = 0.1f;
            }
        }

        void ApplyExtraTurnRotation()
        {

            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }


        public void OnAnimatorMove()
        {


            if (isGrounded && Time.deltaTime > 0)
            {
                Vector3 v = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;


                v.y = rigidbody.velocity.y;
                rigidbody.velocity = v;
            }
        }


        void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR

            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
#endif


            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
            {
                groundNormal = hitInfo.normal;
                isGrounded = true;
                animator.applyRootMotion = true;
            }
            else
            {
                isGrounded = false;
                groundNormal = Vector3.up;
                animator.applyRootMotion = false;
            }
        }
    }
}
